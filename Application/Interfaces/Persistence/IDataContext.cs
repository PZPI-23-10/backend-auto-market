using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Application.Interfaces.Persistence;

public interface IDataContext : IAsyncDisposable
{
    DatabaseFacade Database { get; }
    DbSet<UserAvatar> UserAvatars { get; set; }
    DbSet<EmailVerificationCode> EmailVerificationCodes { get; set; }
    DbSet<VehicleType> VehicleTypes { get; set; }
    DbSet<GearType> GearTypes { get; set; }
    DbSet<FuelType> FuelTypes { get; set; }
    DbSet<VehicleListing> VehicleListings { get; set; }
    DbSet<VehicleBrand> VehicleBrands { get; set; }
    DbSet<VehicleModel> VehicleModels { get; set; }
    DbSet<VehiclePhoto> VehiclePhotos { get; set; }
    DbSet<VehicleCondition> VehicleConditions { get; set; }
    DbSet<VehicleBodyType> BodyTypes { get; set; }
    DbSet<ChatMessage> ChatMessages { get; set; }
    DbSet<Chat> Chats { get; set; }
    DbSet<VehicleModelBodyType> VehicleModelBodyTypes { get; set; }
    DbSet<City> Cities { get; set; }
    DbSet<Region> Regions { get; set; }
    DbSet<User> Users { get; set; }
    DbSet<IdentityUserClaim<int>> UserClaims { get; set; }
    DbSet<IdentityUserLogin<int>> UserLogins { get; set; }
    DbSet<IdentityUserToken<int>> UserTokens { get; set; }
    DbSet<IdentityUserRole<int>> UserRoles { get; set; }
    DbSet<IdentityRole<int>> Roles { get; set; }
    DbSet<IdentityRoleClaim<int>> RoleClaims { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}