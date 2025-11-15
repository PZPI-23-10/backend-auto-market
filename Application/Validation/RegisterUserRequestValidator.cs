using Application.DTOs.Auth;
using Application.Interfaces.Persistence.Repositories;
using FluentValidation;

namespace Application.Validation;

public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
    private readonly IUserRepository _users;

    public RegisterUserRequestValidator(IUserRepository users)
    {
        _users = users;

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Email must be a valid email address.")
            .MustAsync(BeUniqueEmail)
            .WithMessage("User with this email already exists.")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Password)
            .Length(5, 27)
            .WithMessage("Password must be between 5 and 27 characters.")
            .Matches(@"^[a-zA-Z0-9!@#$%^&*]+$")
            .WithMessage("Password can only contain letters, numbers, and special characters.");
    }

    private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
    {
        return !await _users.UserWithEmailExists(email);
    }
}