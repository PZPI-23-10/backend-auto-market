using Application.DTOs.Auth;

namespace Application.DTOs.Listings;

public class OrderedFileDto
{
    public FileDto File { get; set; }
    public int SortOrder { get; set; }
}