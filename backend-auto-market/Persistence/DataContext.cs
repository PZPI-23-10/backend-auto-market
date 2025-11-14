using backend_auto_market.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace backend_auto_market.Persistence;

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