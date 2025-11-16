using Application.Enums;
using Application.Interfaces.Services;

namespace Infrastructure.Services;

public class FileUploadStrategyFactory : IFileUploadStrategyFactory
{
    public IFileUploadStrategy CreateFileUploadStrategy(PhotoCategory category)
    {
        return category switch
        {
            PhotoCategory.Avatar => new AvatarUploadStrategy(),
            PhotoCategory.Listing => new ListingUploadStrategy(),
            _ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
        };
    }
}