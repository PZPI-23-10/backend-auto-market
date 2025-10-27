using System.Net;
using System.Net.Mail;
using backend_auto_market.Configs;
using backend_auto_market.Persistence;
using backend_auto_market.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_auto_market.Services;

public class EmailService
{
    private readonly EmailSettings _configuration;
    private readonly DataContext _context;

    public EmailService(EmailSettings configuration, DataContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    private string GenerateCode() => new Random().Next(100000, 999999).ToString();

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        using var client = new SmtpClient(_configuration.SmtpServer, _configuration.Port)
        {
            Credentials = new NetworkCredential(_configuration.FromEmail, _configuration.Password),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_configuration.FromEmail, _configuration.FromName),
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