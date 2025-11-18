using Application.DTOs;

namespace Application.Interfaces.Services;

public interface IFileStorage
{
    Task<FileUploadResult> Upload(IFileUploadStrategy strategy, Stream stream, string name, int ownerId);
    Task Delete(string publicId);
}