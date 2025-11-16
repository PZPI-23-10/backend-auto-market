namespace Domain.Entities;

public class User : BaseAuditableEntity
{
    public string Email { get; set; }
    public string? Password { get; set; }
    public string FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Country { get; set; }
    public string? AboutYourself { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public bool IsGoogleAuth { get; set; }
    public bool IsVerified { get; set; }
    public virtual ICollection<EmailVerificationCode> EmailVerificationCodes { get; set; } = new List<EmailVerificationCode>();
    public virtual ICollection<VehicleListing> VehicleListings { get; set; } = new List<VehicleListing>();
    public virtual UserAvatar? Avatar { get; set; }
}