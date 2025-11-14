namespace backend_auto_market.Services;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hashedPassword);
}

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool Verify(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}