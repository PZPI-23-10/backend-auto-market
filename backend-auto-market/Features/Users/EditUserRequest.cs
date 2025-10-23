namespace backend_auto_market.Features.Users;

public record EditUserRequest(
    string Password,
    string FirstName,
    string LastName,
    string PhoneNumber,
    DateTime DateOfBirth,
    string UrlPhoto,
    string Address,
    string Country,
    string AboutYourself);