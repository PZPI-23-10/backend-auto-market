using Application.Enums;

namespace Application.Interfaces.Services;

public interface IFileUploadStrategyFactory
{
    IFileUploadStrategy CreateFileUploadStrategy(PhotoCategory category);
}