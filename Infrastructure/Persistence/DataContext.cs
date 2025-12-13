using Application.Interfaces.Persistence;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class DataContext(DbContextOptions options)
    : IdentityDbContext<User, IdentityRole<int>, int>(options), IDataContext
{
    public DbSet<UserAvatar> UserAvatars { get; set; }
    public DbSet<EmailVerificationCode> EmailVerificationCodes { get; set; }

    public DbSet<VehicleType> VehicleTypes { get; set; }
    public DbSet<GearType> GearTypes { get; set; }
    public DbSet<FuelType> FuelTypes { get; set; }
    public DbSet<VehicleListing> VehicleListings { get; set; }
    public DbSet<VehicleBrand> VehicleBrands { get; set; }
    public DbSet<VehicleModel> VehicleModels { get; set; }
    public DbSet<VehiclePhoto> VehiclePhotos { get; set; }
    public DbSet<VehicleCondition> VehicleConditions { get; set; }
    public DbSet<VehicleBodyType> BodyTypes { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<VehicleModelBodyType> VehicleModelBodyTypes { get; set; }

    public DbSet<City> Cities { get; set; }
    public DbSet<Region> Regions { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditableEntities()
    {
        var entries = ChangeTracker.Entries<IAuditableEntity>();

        foreach (var e in entries)
        {
            var now = DateTime.UtcNow;

            if (e.State == EntityState.Added)
            {
                e.Property(x => x.Created).CurrentValue = now;
                e.Property(x => x.LastModified).CurrentValue = now;
            }
            else if (e.State == EntityState.Modified)
            {
                e.Property(x => x.LastModified).CurrentValue = now;
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();

            entity.HasMany(u => u.EmailVerificationCodes)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(u => u.VehicleListings)
                .WithOne(vl => vl.User)
                .HasForeignKey(vl => vl.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<VehicleListing>(entity =>
        {
            entity.Property(vl => vl.Price)
                .HasPrecision(18, 2);

            entity.HasMany(vl => vl.Photos)
                .WithOne(vp => vp.VehicleListing)
                .HasForeignKey(vp => vp.VehicleListingId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<VehicleModelBodyType>(entity =>
        {
            entity.HasKey(t => new { t.VehicleModelId, t.BodyTypeId });

            entity.HasOne(t => t.VehicleModel)
                .WithMany(x => x.VehicleModelBodyTypes)
                .HasForeignKey(t => t.VehicleModelId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(t => t.BodyType)
                .WithMany()
                .HasForeignKey(t => t.BodyTypeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasIndex(r => r.Name).IsUnique();

            entity.HasMany(r => r.Cities)
                .WithOne(c => c.Region)
                .HasForeignKey(c => c.RegionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasIndex(c => new { c.RegionId, c.Name }).IsUnique();

            entity.HasMany<VehicleListing>()
                .WithOne(vl => vl.City)
                .HasForeignKey(vl => vl.CityId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<VehicleBrand>(entity =>
        {
            entity.HasIndex(b => b.Name).IsUnique();

            entity.HasMany(b => b.VehicleModels)
                .WithOne(vm => vm.Brand)
                .HasForeignKey(vm => vm.BrandId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<VehicleModel>(entity =>
        {
            entity.HasMany<VehicleListing>()
                .WithOne(vl => vl.Model)
                .HasForeignKey(vl => vl.ModelId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<VehicleBodyType>(entity =>
        {
            entity.HasIndex(bt => bt.Name).IsUnique();

            entity.HasMany<VehicleListing>()
                .WithOne(vl => vl.BodyType)
                .HasForeignKey(vl => vl.BodyTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<VehicleCondition>(entity =>
        {
            entity.HasIndex(c => c.Name).IsUnique();

            entity.HasMany<VehicleListing>()
                .WithOne(vl => vl.Condition)
                .HasForeignKey(vl => vl.ConditionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<FuelType>(entity =>
        {
            entity.HasIndex(c => c.Name).IsUnique();

            entity.HasMany<VehicleListing>()
                .WithOne(x => x.FuelType)
                .HasForeignKey(x => x.FuelTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<GearType>(entity =>
        {
            entity.HasIndex(c => c.Name).IsUnique();

            entity.HasMany<VehicleListing>()
                .WithOne(x => x.GearType)
                .HasForeignKey(x => x.GearTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<VehicleType>(entity =>
        {
            entity.HasIndex(c => c.Name).IsUnique();

            entity.HasMany(x => x.Models)
                .WithOne(x => x.VehicleType)
                .HasForeignKey(x => x.VehicleTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });


        modelBuilder.Entity<Chat>(e =>
        {
            e.HasKey(c => c.Id);

            e.HasOne(c => c.FirstUser)
                .WithMany()
                .HasForeignKey(c => c.FirstUserId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(c => c.SecondUser)
                .WithMany()
                .HasForeignKey(c => c.SecondUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ChatMessage>(e =>
        {
            e.HasOne(m => m.Chat)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChatId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            e.Property(m => m.Text).IsRequired().HasMaxLength(2000);
            e.Property(m => m.Created).IsRequired();
        });
    }
}