using System.Net;
using System.Net.Mail;
using backend_auto_market.Persistence;
using backend_auto_market.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_auto_market.Services;

public class EmailService
{
    private readonly IConfiguration _configuration;
    private readonly DataContext _context;
    private readonly string _fromEmail;
    private readonly string _fromName;
    private readonly string _password;
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly bool _enableSsl;

    public EmailService(IConfiguration configuration, DataContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    private string GenerateCode() => new Random().Next(100000, 999999).ToString();

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        using var client = new SmtpClient(_smtpServer, _smtpPort)
        {
            Credentials = new NetworkCredential(_fromEmail, _password),
            EnableSsl = _enableSsl
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_fromEmail, _fromName),
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