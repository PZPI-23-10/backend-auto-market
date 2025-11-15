using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class VerificationCodeRepository(DataContext context)
    : BaseRepository<EmailVerificationCode>(context), IVerificationCodeRepository
{
    public async Task<EmailVerificationCode?> GetByUserId(int userId)
    {
        return await DataContext.Set<EmailVerificationCode>().FirstOrDefaultAsync(x => x.UserId == userId);
    }
}