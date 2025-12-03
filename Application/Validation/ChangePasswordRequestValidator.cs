using Application.DTOs.Auth;
using FluentValidation;

namespace Application.Validation;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.PasswordConfirmation)
            .Equal(x => x.NewPassword)
            .WithMessage("New password and confirmation do not match.");
    }
}