using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SubFlow.Api.Domain.Entities;
using SubFlow.Api.Features.Auth.Common;
using SubFlow.Api.Infrastructure.Persistence;
using SubFlow.Api.Infrastructure.Services;

namespace SubFlow.Api.Features.Auth.Commands
{
    public class RefreshTokenCommandHandler(
    AppDbContext context,
    ITokenService tokenService,
    IOptions<JwtSettings> jwtOptions) : IRequestHandler<RefreshTokenCommand, AuthResponse>
    {
        private readonly JwtSettings _jwtSettings = jwtOptions.Value;

        public async Task<AuthResponse> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
        {
            var existingToken = await context.RefreshTokens
    .Include(rt => rt.User)
    .FirstOrDefaultAsync(rt => rt.Token == command.RefreshToken, cancellationToken);

            if (existingToken == null)
            {
                throw new UnauthorizedAccessException("Invalid token.");
            }
            if (existingToken.IsRevoked && !string.IsNullOrEmpty(existingToken.ReplacedByToken))
            {
                // НАКАЗАНИЕ: сбрасываем ВСЕ сессии этого пользователя!
                var userTokens = await context.RefreshTokens
                    .Where(rt => rt.UserId == existingToken.UserId && rt.RevokedAtUtc == null)
                    .ToListAsync(cancellationToken);

                foreach (var token in userTokens)
                {
                    token.RevokedAtUtc = DateTime.UtcNow;
                }

                await context.SaveChangesAsync(cancellationToken);

                throw new SecurityTokenException("Security alert: Compromised token reuse detected. All sessions terminated.");
            }
            if (existingToken.IsActive == false)
            {
                throw new UnauthorizedAccessException("Token is expired or inactive.");
            }
            var newRefreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = existingToken.UserId, // ✅ Берем ID юзера из найденного токена!
                Token = tokenService.GenerateRefreshToken(),
                CreatedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays)
            };
            existingToken.RevokedAtUtc = DateTime.UtcNow;
            existingToken.ReplacedByToken = newRefreshToken.Token;

            context.RefreshTokens.Add(newRefreshToken);     
            await context.SaveChangesAsync(cancellationToken); 

            var accessToken = tokenService.GenerateAccessToken(existingToken.User);

            return new AuthResponse(accessToken, newRefreshToken.Token, newRefreshToken.ExpiresAtUtc);
        }
    }
}