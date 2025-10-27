namespace backend_auto_market.Configs;

public class SendGridSettings
{
    public string ApiKey { get; set; } = null!;
    public string FromEmail { get; set; } = null!;
    public string FromName { get; set; } = "AutoMarket";
}