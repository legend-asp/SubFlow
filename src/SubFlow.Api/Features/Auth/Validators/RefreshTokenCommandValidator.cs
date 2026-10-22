using FluentValidation;
using SubFlow.Api.Features.Auth.Commands;

namespace SubFlow.Api.Features.Auth.Validators
{
    public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator() 
        {
            RuleFor(u => u.RefreshToken)
                    .NotEmpty().WithMessage("Refresh token is required.");
        }
    }
}