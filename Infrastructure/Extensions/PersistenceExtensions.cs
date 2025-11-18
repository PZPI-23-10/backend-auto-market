using Application.Interfaces.Persistence;
using Application.Interfaces.Persistence.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services,
        IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<DataContext>(options => options.UseNpgsql(connectionString)
            .UseAsyncSeeding(DataSeeder.Seed)
            .UseSeeding((context, cancellationToken) =>
                DataSeeder.Seed(context, cancellationToken)
                    .GetAwaiter()
                    .GetResult()
            )
            .UseLazyLoadingProxies()
        );

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IVerificationCodeRepository, VerificationCodeRepository>();

        services.AddScoped<IVehicleListingRepository, VehicleListingRepository>();
        services.AddScoped<IVehicleBrandRepository, VehicleBrandRepository>();
        services.AddScoped<IVehicleModelRepository, VehicleModelRepository>();
        services.AddScoped<IVehicleBodyTypeRepository, VehicleBodyTypeRepository>();
        services.AddScoped<IVehicleConditionRepository, VehicleConditionRepository>();
        services.AddScoped<IVehiclePhotoRepository, VehiclePhotoRepository>();
        services.AddScoped<IVehicleTypeRepository, VehicleTypeRepository>();
        services.AddScoped<IGearTypeRepository, GearTypeRepository>();
        services.AddScoped<IFuelTypeRepository, FuelTypeRepository>();

        services.AddScoped<IRegionRepository, RegionRepository>();
        services.AddScoped<ICityRepository, CityRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}