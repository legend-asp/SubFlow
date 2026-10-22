using MediatR;
using Microsoft.EntityFrameworkCore;
using SubFlow.Api.Infrastructure.Persistence;

namespace SubFlow.Api.Features.Auth.Commands
{
    public class RevokeTokenCommandHandler(
        AppDbContext context
        ) : IRequestHandler<RevokeTokenCommand>
    {
        public async Task Handle( RevokeTokenCommand command, CancellationToken cancellationToken )
        {
            var token = await context.RefreshTokens
           .FirstOrDefaultAsync(rt => rt.Token == command.RefreshToken, cancellationToken);

            if (token == null || token.IsRevoked)
            {
                return;
            }
            else
            {
                token.RevokedAtUtc = DateTime.UtcNow;
                await context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}