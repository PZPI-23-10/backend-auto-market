using System.Net;
using System.Net.Mail;
using System.Text.Json;
using backend_auto_market.Configs;
using Microsoft.Extensions.Options;
using SmtpClient = System.Net.Mail.SmtpClient;
namespace backend_auto_market.Services;

public class EmailService(IOptions<SendGridSettings> options)
{
    private readonly SendGridSettings _config = options.Value;
    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        using var client = new SmtpClient("smtp.sendgrid.net", 587)
        {
            Credentials = new NetworkCredential("apikey", _config.ApiKey),
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_config.FromEmail, _config.FromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mailMessage.To.Add(toEmail);

        // ✅ Добавляем X-SMTPAPI (если хочешь использовать категории, трекинг и т.д.)
        var smtpApiHeader = new
        {
            category = "AutoMarket",
            filters = new
            {
                clicktrack = new { settings = new { enable = 0 } },
                opentrack = new { settings = new { enable = 1 } }
            }
        };

        string smtpApiJson = JsonSerializer.Serialize(smtpApiHeader);
        mailMessage.Headers.Add("X-SMTPAPI", smtpApiJson);

        await client.SendMailAsync(mailMessage);
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