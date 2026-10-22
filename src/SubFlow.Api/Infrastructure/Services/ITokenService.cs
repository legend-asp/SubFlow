using SubFlow.Api.Domain.Entities;

namespace SubFlow.Api.Infrastructure.Services;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}