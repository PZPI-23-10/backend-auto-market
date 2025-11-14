using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class DataContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<EmailVerificationCode> EmailVerificationCodes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasMany(user => user.EmailVerificationCodes)
            .WithOne(emailVerificationCode => emailVerificationCode.User)
            .HasForeignKey(emailVerificationCode => emailVerificationCode.UserId);
    }
}