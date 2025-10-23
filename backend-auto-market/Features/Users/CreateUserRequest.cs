namespace backend_auto_market.Features.Users;

public record CreateUserRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Country,
    DateTime DateOfBirth,
    string? Address = null
);