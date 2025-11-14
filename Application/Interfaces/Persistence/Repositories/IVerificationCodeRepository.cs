using Domain.Entities;

namespace Application.Interfaces.Persistence.Repositories;

public interface IVerificationCodeRepository : IBaseRepository<EmailVerificationCode>
{
    Task<EmailVerificationCode?> GetByUserId(int userId);
}