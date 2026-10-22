using MediatR;
using SubFlow.Api.Features.Auth.Common;

namespace SubFlow.Api.Features.Auth.Commands
{
        public record LoginCommand(string Email, string Password) : IRequest<AuthResponse>;
    
}
