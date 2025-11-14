using System.Net;
using System.Net.Mail;
using Application.Interfaces.Services;

namespace Infrastructure.Services;

public class EmailSender(EmailSettings configuration) : IEmailSender
{
    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        using var client = new SmtpClient(configuration.SmtpServer, configuration.Port);
        client.UseDefaultCredentials = false;
        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        client.Credentials = new NetworkCredential(configuration.FromEmail, configuration.Password);
        client.EnableSsl = true;

        var mailMessage = new MailMessage
        {
            From = new MailAddress(configuration.FromEmail, configuration.FromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mailMessage.To.Add(toEmail);
        await client.SendMailAsync(mailMessage);
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

    public async Task SendPasswordChangedInfo(string toEmail, string newPassword)
    {
        string subject = "Зміна пароля — AutoMarket";
        string body = $"""
                       <h3>Пароль змінено</h3>
                       <p>Новий пароль:</p>
                       <h2 style="color:#007bff;">{newPassword}</h2>
                       """;
    }
}