namespace Application.Interfaces.Services;

public interface IFileStorage
{
    Task<string> Upload(IFileUploadStrategy strategy, Stream stream, string name, int ownerId);
}