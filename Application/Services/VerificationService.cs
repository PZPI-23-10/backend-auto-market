using System.ComponentModel.DataAnnotations;
using Application.Exceptions;
using Application.Interfaces.Persistence;
using Application.Interfaces.Persistence.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class VerificationService(
    IVerificationCodeRepository verifications,
    IEmailSender emailSender,
    IUserRepository users,
    IUnitOfWork unitOfWork
)
    : IVerificationService
{
    public async Task SendRegisterCode(User user)
    {
        string code = await EnsureVerificationCode(user, VerificationType.Register);

        await SendRegistrationEmail(user.FirstName, user.Email, code);
    }

    public async Task ResendRegisterCode(int userId)
    {
        var user = await users.GetByIdAsync(userId);

        if (user == null)
            throw new NotFoundException("User not found.");

        if (user.IsVerified)
            throw new ValidationException("Email already verified.");

        string code = await EnsureVerificationCode(user, VerificationType.Register);

        await SendRegistrationEmail(user.FirstName, user.Email, code);

        await unitOfWork.SaveChangesAsync();
    }

    public async Task VerifyCode(int userId, string code)
    {
        User? user = await users.GetByIdAsync(userId);

        if (user == null)
            throw new NotFoundException("User not found");

        EmailVerificationCode? verification = await verifications.GetByUserId(userId);

        if (verification == null)
            throw new NotFoundException("Email verification code not found.");

        if (verification.Code != code)
            throw new ValidationException("Invalid verification code.");

        if (verification.ExpirationTime < DateTime.UtcNow)
            throw new ValidationException("Verification code has expired.");

        user.IsVerified = true;

        verifications.Remove(verification);
        users.Update(user);

        await unitOfWork.SaveChangesAsync();
    }

    private async Task<string> EnsureVerificationCode(User user, VerificationType type)
    {
        string code = new Random().Next(100000, 999999).ToString();

        var existingCode = await verifications.GetByUserId(user.Id);

        if (existingCode != null)
        {
            existingCode.Code = code;
            existingCode.ExpirationTime = DateTime.UtcNow.AddMinutes(15);
            existingCode.Type = type;
            verifications.Update(existingCode);
        }
        else
        {
            EmailVerificationCode verificationCode = new EmailVerificationCode
            {
                Code = code,
                ExpirationTime = DateTime.UtcNow.AddMinutes(15),
                Type = type,
                User = user
            };
            await verifications.AddAsync(verificationCode);
        }


        return code;
    }

    private async Task SendRegistrationEmail(string name, string email, string code)
    {
        string subject = "Підтвердження реєстрації — AutoMarket";
        string body = $"""
                       <h2>Вітаю, {name}!</h2>
                       <p>Ваш код підтвердження:</p>
                       <h2 style="color:#007bff;">{code}</h2>
                       <p>Введіть цей код на сайті. Термін дії — 15 хвилин.</p>
                       """;

        await emailSender.SendEmailAsync(email, subject, body);
    }
}