namespace Application.Interfaces.Services;

public interface IFileHashService
{
    string ComputeHash(Stream fileStream);
}