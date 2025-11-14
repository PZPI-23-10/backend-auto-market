namespace backend_auto_market.Services;

public interface IFileStorage
{
    Task<string> UploadAvatar(Stream fileStream, string fileName, int userId);
}