using CloudinaryDotNet;

namespace Application.Interfaces.Services;

public interface IFileUploadStrategy
{
    string BuildPath(int directoryId);
    Transformation BuildTransformation();
}