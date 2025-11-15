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
}