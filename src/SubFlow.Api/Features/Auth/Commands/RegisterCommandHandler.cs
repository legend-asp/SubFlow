using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SubFlow.Api.Domain.Entities;
using SubFlow.Api.Features.Auth.Common;
using SubFlow.Api.Infrastructure.Persistence;
using SubFlow.Api.Infrastructure.Services;

namespace SubFlow.Api.Features.Auth.Commands;

public class RegisterCommandHandler(
    AppDbContext context,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IOptions<JwtSettings> jwtOptions) : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // 1. Проверяем, свободен ли Email
        var emailExists = await context.Users
            .AnyAsync(u => u.Email == request.Email, cancellationToken);

        if (emailExists)
        {
            throw new InvalidOperationException("User with this email already exists.");
        }

        // 2. Хэшируем пароль
        var passwordHash = passwordHasher.Hash(request.Password);

        // 3. Создаем сущность User
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = passwordHash,
            CreatedAtUtc = DateTime.UtcNow
        };

        // 4. Генерируем первый Refresh Token
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = tokenService.GenerateRefreshToken(),
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays)
        };

        user.RefreshTokens.Add(refreshToken);

        // 5. Сохраняем в PostgreSQL одной транзакцией
        context.Users.Add(user);
        await context.SaveChangesAsync(cancellationToken);

        // 6. Генерируем JWT Access Token
        var accessToken = tokenService.GenerateAccessToken(user);

        return new AuthResponse(
            accessToken,
            refreshToken.Token,
            refreshToken.ExpiresAtUtc
        );
    }
}