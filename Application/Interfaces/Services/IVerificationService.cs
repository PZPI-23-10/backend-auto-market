using Domain.Entities;

namespace Application.Interfaces.Services;

public interface IVerificationService
{
    Task SendRegisterCode(User user);
    Task ResendRegisterCode(int userId);
    Task VerifyCode(int userId, string code);
}