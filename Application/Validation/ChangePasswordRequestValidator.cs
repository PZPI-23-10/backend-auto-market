using Application.DTOs.Auth;
using FluentValidation;

namespace Application.Validation;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.NewPassword)
            .Length(5, 27)
            .WithMessage("Password must be between 5 and 27 characters.")
            .Matches(@"^[a-zA-Z0-9!@#$%^&*]+$")
            .WithMessage("Password can only contain letters, numbers, and special characters.");

        RuleFor(x => x.PasswordConfirmation)
            .Equal(x => x.NewPassword)
            .WithMessage("New password and confirmation do not match.");
    }
}