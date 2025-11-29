namespace Application.DTOs.Profile;

public class UserProfileResponse
{
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Country { get; set; }
    public string? AboutUrself { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public bool? IsGoogleAuth { get; set; }
    public bool? IsVerified { get; set; }
    public string? AvatarUrl { get; set; }
    public IEnumerable<string>? Roles { get; set; }
}