using Application.Interfaces.Services;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace Infrastructure.Services;

public class CloudinaryFileStorage(Cloudinary cloudinary) : IFileStorage
{
    public async Task<string> Upload(IFileUploadStrategy strategy, Stream stream, string name, int ownerId)
    {
        string path = strategy.BuildPath(ownerId);
        Transformation transformation = strategy.BuildTransformation();

        ImageUploadParams uploadParams = new ImageUploadParams
        {
            PublicId = path,
            File = new FileDescription(name, stream),
            Transformation = transformation
        };

        ImageUploadResult uploadResult = await cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error != null)
        {
            throw new Exception($"Cloudinary upload error: {uploadResult.Error.Message}");
        }

        return uploadResult.SecureUrl.ToString();
    }
}