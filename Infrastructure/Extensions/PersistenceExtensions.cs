using Application.Interfaces.Persistence;
using Application.Interfaces.Persistence.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
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

        services
            .AddDbContext<DataContext>(options => options.UseNpgsql(connectionString))
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IVerificationCodeRepository, VerificationCodeRepository>()
            .AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }   
}