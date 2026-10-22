using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using SubFlow.Api.Domain.Entities;
using SubFlow.Api.Features.Auth.Common;
using SubFlow.Api.Infrastructure.Persistence;
using SubFlow.Api.Infrastructure.Services;
using System.Linq;

namespace SubFlow.Api.Features.Auth.Commands
{
    public class LoginCommandHandler(
        AppDbContext context,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IOptions<JwtSettings> jwtOptions) : IRequestHandler<LoginCommand, AuthResponse>
    {
        private readonly JwtSettings _jwtSettings = jwtOptions.Value;

        public async Task<AuthResponse> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            var user = await context.Users
                .FirstOrDefaultAsync(x => x.Email == command.Email, cancellationToken);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }
            var isPasswordValid = passwordHasher.Verify(command.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = tokenService.GenerateRefreshToken(),
                CreatedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays)
            };

            var accessToken = tokenService.GenerateAccessToken(user);

            user.RefreshTokens.Add(refreshToken);

            await context.SaveChangesAsync(cancellationToken);

            return new AuthResponse(accessToken, refreshToken.Token, refreshToken.ExpiresAtUtc);
        }
    }
}
