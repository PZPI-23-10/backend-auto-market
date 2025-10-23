namespace backend_auto_market.Persistence.Models;

public class User : BaseEntity
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string PhoneNumber { get; set; }
    public string Country { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }

    public string? About { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Address { get; set; }

    public bool IsGoogleAuth { get; set; } = false;
}