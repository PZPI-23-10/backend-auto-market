namespace Application.Interfaces.Services;

public interface IFileStorage
{
    Task<string> UploadAvatar(Stream fileStream, string fileName, int userId);
}