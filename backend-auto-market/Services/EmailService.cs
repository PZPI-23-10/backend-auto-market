using System.Net;
using System.Net.Mail;
using backend_auto_market.Configs;
using Microsoft.Extensions.Options;

namespace backend_auto_market.Services;

public class EmailService(IOptions<EmailSettings> options)
{
    private EmailSettings Configuration => options.Value;

    private string GenerateCode() => new Random().Next(100000, 999999).ToString();

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        using var client = new SmtpClient(Configuration.SmtpServer, Configuration.Port);
        client.UseDefaultCredentials = false;
        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        client.Credentials = new NetworkCredential(Configuration.FromEmail, Configuration.Password);
        client.EnableSsl = true;

        var mailMessage = new MailMessage
        {
            From = new MailAddress(Configuration.FromEmail, Configuration.FromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mailMessage.To.Add(toEmail);
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