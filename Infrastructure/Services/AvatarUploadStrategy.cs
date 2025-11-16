using Application.Interfaces.Services;
using CloudinaryDotNet;

namespace Infrastructure.Services;

public class AvatarUploadStrategy : IFileUploadStrategy
{
    public string BuildPath(int directoryId) => $"avatars/{directoryId}/{Guid.NewGuid()}";

    public Transformation BuildTransformation() =>
        new Transformation().Width(500).Height(500).Crop("fill").Gravity("face");
}