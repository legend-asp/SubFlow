namespace SubFlow.Api.Features.Auth.Common
{
    public record AuthResponse(
        string AccessToken,
        string RefreshToken,
        DateTime RefreshTokenExpiresAtUtc
        );
    
}
