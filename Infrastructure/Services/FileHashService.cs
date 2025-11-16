using System.Security.Cryptography;
using Application.Interfaces.Services;

namespace Infrastructure.Services;

public class FileHashService : IFileHashService
{
    public string ComputeHash(Stream fileStream)
    {
        using var sha = SHA256.Create();
        fileStream.Position = 0;
        byte[] hashBytes = sha.ComputeHash(fileStream);
        fileStream.Position = 0;
        return Convert.ToBase64String(hashBytes);
    }
}