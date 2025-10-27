using System.Net;
using System.Net.Mail;
using backend_auto_market.Configs;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace backend_auto_market.Services;

public class EmailService(IOptions<EmailSettings> options)
{
    private EmailSettings _settings => options.Value;
    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var mailMessage = new MimeMessage();
        mailMessage.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
        mailMessage.To.Add(MailboxAddress.Parse(toEmail));
        mailMessage.Subject = subject;
        mailMessage.Body = new TextPart("html") { Text = body };

        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.SmtpServer, _settings.Port, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_settings.Username, _settings.Password);
        await client.SendAsync(mailMessage);
        await client.DisconnectAsync(true);
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