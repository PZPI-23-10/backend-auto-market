namespace backend_auto_market.Features.Users;

public record RegisterUserRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Country,
    string AboutYourself,
    DateTime DateOfBirth
    );