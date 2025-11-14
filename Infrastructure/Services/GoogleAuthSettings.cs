namespace Infrastructure.Services;

[Serializable]
public class GoogleAuthSettings
{
    public string WebClientId { get; set; }
    public string AndroidClientId { get; set; }
    public string ClientSecret { get; set; }
}