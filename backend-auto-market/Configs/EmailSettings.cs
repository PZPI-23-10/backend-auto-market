namespace backend_auto_market.Configs;

[Serializable]
public class EmailSettings
{
    public string FromEmail { get; set; }
    public string FromName { get; set; }
    public string SmtpServer { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}