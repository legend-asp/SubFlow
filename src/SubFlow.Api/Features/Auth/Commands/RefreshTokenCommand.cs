using MediatR;
using SubFlow.Api.Features.Auth.Common;

namespace SubFlow.Api.Features.Auth.Commands
{
    public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResponse>;
}
