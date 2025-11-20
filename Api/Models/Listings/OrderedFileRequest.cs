namespace Api.Models.Listings;

public class OrderedFileRequest
{
    public IFormFile? File { get; set; }
    public int SortOrder { get; set; }
}