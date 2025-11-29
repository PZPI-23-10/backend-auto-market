using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class User : IdentityUser<int>, IAuditableEntity
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Country { get; set; }
    public string? AboutYourself { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public bool IsGoogleAuth { get; set; }
    public virtual ICollection<EmailVerificationCode> EmailVerificationCodes { get; set; } = new List<EmailVerificationCode>();
    public virtual ICollection<VehicleListing> VehicleListings { get; set; } = new List<VehicleListing>();
    public virtual UserAvatar? Avatar { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset LastModified { get; set; }
}