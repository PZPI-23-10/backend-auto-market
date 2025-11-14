using Application.DTOs.Auth;
using FluentValidation;

namespace Application.Validation;

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .Length(5, 27)
            .WithMessage("Password must be between 5 and 27 characters.")
            .Matches(@"^[a-zA-Z0-9!@#$%^&*]+$")
            .WithMessage("Password can only contain letters, numbers, and special characters.");

        RuleFor(x => x.PasswordConfirmation)
            .Equal(x => x.Password)
            .WithMessage("Password confirmation must match.");
    }
}