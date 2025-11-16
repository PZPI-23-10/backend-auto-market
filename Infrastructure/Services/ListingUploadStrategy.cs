using Application.Interfaces.Services;
using CloudinaryDotNet;

namespace Infrastructure.Services;

public class ListingUploadStrategy : IFileUploadStrategy
{
    public string BuildPath(int directoryId) => $"listings/{directoryId}/{Guid.NewGuid()}";

    public Transformation BuildTransformation() => new Transformation().Width(1200).Height(900).Crop("fill");
}