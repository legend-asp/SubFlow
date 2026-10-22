using MediatR;
using SubFlow.Api.Features.Auth.Common;
using System.ComponentModel.DataAnnotations;

namespace SubFlow.Api.Features.Auth.Commands
{
    public record RegisterCommand(string Email, string Password) : IRequest<AuthResponse>;
    
}
