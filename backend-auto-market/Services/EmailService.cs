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

    public async Task<string> CreateAndSendVerificationCodeAsync(
        string email,
        VerificationType type,
        string? firstName = null)
    {
        string code = GenerateCode();

        var verification = new EmailVerificationCode
        {
            Email = email,
            Code = code,
            ExpirationTime = DateTime.UtcNow.AddMinutes(15),
            Type = type
        };

        _context.EmailVerificationCodes.Add(verification);
        await _context.SaveChangesAsync();
        
        string subject;
        string body;

        switch (type)
        {
            case VerificationType.Register:
                subject = "Підтвердження реєстрації на AutoMarket";
                body = $"""
                            <h2>Вітаю, {firstName ?? "користувачу"}!</h2>
                            <p>Ваш код для підтвердження реєстрації:</p>
                            <h1 style="color:#007bff;">{code}</h1>
                            <p>Код дійсний 15 хвилин.</p>
                        """;
                break;

            case VerificationType.PasswordReset:
                subject = "Відновлення паролю — AutoMarket";
                body = $"""
                            <h3>Ваш код для відновлення паролю:</h3>
                            <h1 style="color:#007bff;">{code}</h1>
                            <p>Код дійсний 15 хвилин.</p>
                        """;
                break;

            case VerificationType.PasswordChange:
                subject = "Підтвердження зміни паролю — AutoMarket";
                body = $"""
                            <h3>Ваш код для підтвердження зміни паролю:</h3>
                            <h1 style="color:#007bff;">{code}</h1>
                            <p>Код дійсний 15 хвилин.</p>
                        """;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
        
        await SendEmailAsync(email, subject, body);
        return code;
    }
    
    public async Task<bool> VerifyCodeAsync(string email, string code, VerificationType type)
    {
        var record = await _context.EmailVerificationCodes
            .Where(v => v.Email == email && v.Code == code && v.Type == type)
            .OrderByDescending(v => v.ExpirationTime)
            .FirstOrDefaultAsync();

        if (record == null || record.ExpirationTime < DateTime.UtcNow)
            return false;
        
        _context.EmailVerificationCodes.Remove(record);
        await _context.SaveChangesAsync();

        return true;
    }
    
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