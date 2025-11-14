using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace backend_auto_market.Services;

public class CloudinaryFileStorage(Cloudinary cloudinary) : IFileStorage
{
    public async Task<string> UploadAvatar(Stream fileStream, string fileName, int userId)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, fileStream),

            PublicId = $"avatars/{userId}/{Guid.NewGuid()}",

            Transformation = new Transformation()
                .Width(500).Height(500).Crop("fill").Gravity("face")
        };

        ImageUploadResult? uploadResult = await cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error != null)
        {
            throw new Exception($"Cloudinary upload error: {uploadResult.Error.Message}");
        }

        return uploadResult.SecureUrl.ToString();
    }
}