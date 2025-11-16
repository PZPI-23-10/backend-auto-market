using Application.Interfaces.Services;
using CloudinaryDotNet;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        EmailSettings emailSettings = configuration.GetSection("EmailSettings").Get<EmailSettings>() ??
                                      throw new NullReferenceException("Email settings not found");

        CloudinarySettings cloudinarySettings =
            configuration.GetSection("CloudinarySettings").Get<CloudinarySettings>() ??
            throw new NullReferenceException("Cloudinary settings not found");

        GoogleAuthSettings googleAuthSettings =
            configuration.GetSection("GoogleAuthSettings").Get<GoogleAuthSettings>() ??
            throw new NullReferenceException("Google auth settings not found");

        Account account = new Account(
            cloudinarySettings.CloudName,
            cloudinarySettings.ApiKey,
            cloudinarySettings.ApiSecret
        );

        Cloudinary cloudinary = new Cloudinary(account);

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IFileStorage, CloudinaryFileStorage>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddScoped<IUrlSafeEncoder, UrlSafeEncoder>();
        services.AddScoped<IGoogleTokenValidator, GoogleTokenValidator>();
        services.AddScoped<IFileHashService, FileHashService>();

        services.AddTransient<IFileUploadStrategyFactory, FileUploadStrategyFactory>();

        services.AddSingleton(emailSettings);
        services.AddSingleton(cloudinarySettings);
        services.AddSingleton(cloudinary);
        services.AddSingleton(googleAuthSettings);

        return services;
    }
}