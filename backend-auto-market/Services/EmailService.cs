using System.Net;
using System.Net.Mail;
using backend_auto_market.Configs;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using SmtpClient = System.Net.Mail.SmtpClient;

namespace backend_auto_market.Services;

public class EmailService(IOptions<EmailSettings> options)
{
    private EmailSettings Configuration => options.Value;
    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        using var client = new SmtpClient("smtp.gmail.com");
        client.UseDefaultCredentials = false;
        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        client.Credentials = new NetworkCredential("automarket.noreply@gmail.com", "clws vuaj mjhz jkqf");
        client.EnableSsl = true;

        var mailMessage = new MailMessage
        {
            From = new MailAddress("automarket.noreply@gmail.com", "automarket.noreply@gmail.com"),
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