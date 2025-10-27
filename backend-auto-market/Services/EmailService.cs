using System.Text;
using backend_auto_market.Configs;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using backend_auto_market.Configs;
using Microsoft.Extensions.Options;
namespace backend_auto_market.Services;

public class EmailService(IOptions<SendGridSettings> options)
{
    private readonly SendGridSettings _config = options.Value;
    private readonly SendGridClient _client = new(options.Value.ApiKey);

    public async Task SendEmailAsync(string toEmail, string subject, string htmlContent, string? plainText = null)
    {
        var from = new EmailAddress(_config.FromEmail, _config.FromName);
        var to = new EmailAddress(toEmail);
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainText ?? htmlContent, htmlContent);

        var response = await _client.SendEmailAsync(msg);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Body.ReadAsStringAsync();
            throw new Exception($"SendGrid error: {response.StatusCode} - {error}");
        }
    }

    public async Task SendRegistrationEmail(string toEmail, string firstName, string code)
    {
        string subject = "Підтвердження реєстрації — AutoMarket";
        string body = $"""
                       <h2>Вітаю, {firstName}!</h2>
                       <p>Ваш код підтвердження:</p>
                       <h2 style="color:#007bff;">{code}</h2>
                       <p>Введіть цей код на сайті. Термін дії — 15 хвилин.</p>
                       """;
        await SendEmailAsync(toEmail, subject, body);
    }

    public async Task SendPasswordRecoveryEmail(string toEmail, string code)
    {
        string subject = "Відновлення пароля — AutoMarket";
        string body = $"""
                       <h3>Відновлення доступу</h3>
                       <p>Ваш код для скидання пароля:</p>
                       <h2 style="color:#007bff;">{code}</h2>
                       <p>Термін дії — 15 хвилин.</p>
                       """;
        await SendEmailAsync(toEmail, subject, body);
    }
}