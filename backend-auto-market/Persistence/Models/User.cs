namespace backend_auto_market.Persistence.Models;

public class User : BaseEntity
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string Country { get; set; }
    public string AboutYourself { get; set; }
    public DateTime DateOfBirth { get; set; }
}