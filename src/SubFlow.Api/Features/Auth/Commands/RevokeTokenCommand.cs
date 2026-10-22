using MediatR;

namespace SubFlow.Api.Features.Auth.Commands;

public record RevokeTokenCommand(string RefreshToken) : IRequest;
