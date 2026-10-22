using FluentValidation;
using Microsoft.AspNetCore.Rewrite;
using SubFlow.Api.Features.Auth.Commands;

namespace SubFlow.Api.Features.Auth.Validators
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Email)
                    .NotEmpty().WithMessage("Email cant be empty")
                    .EmailAddress().WithMessage("invalid email adress");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password cant be empty");
        }
    }
}
