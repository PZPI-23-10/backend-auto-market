using Api.Extensions;
using Api.Hubs;
using Application.Extensions;
using Application.Interfaces.Persistence;
using Application.Interfaces.Services;
using Application.Services;
using Application.Validation;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;

namespace Api;

public static class Program
{
    public static async Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddJwtAuthentication(builder.Configuration);
        builder.Services.AddAuthorization();

        builder.Services.AddIdentityCore<User>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
            })
            .AddRoles<IdentityRole<int>>()
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<DataContext>();

        builder.Services.AddSignalR();

        builder.Services.AddApiControllers();
        builder.Services.AddScoped<IFavouriteService, FavouriteService>();
        builder.Services.AddValidatorsFromAssembly(typeof(RegisterUserRequestValidator).Assembly);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerDocumentation();

        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddApplicationServices();

        builder.Services.AddPersistence(builder.Configuration);

        builder.Services.AddHttpClient("BazaGaiClient", client =>
            {
                client.BaseAddress = new Uri("https://baza-gai.com.ua/nomer/");
                client.Timeout = TimeSpan.FromSeconds(30);

                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("X-Api-Key", "62fee967df633514fcee3765522226dc");

                client.DefaultRequestHeaders.UserAgent.ParseAdd(
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();

                handler.AllowAutoRedirect = true;

                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

                return handler;
            });
        builder.Services
            .AddScoped<Application.Interfaces.Services.IVehicleVerificationService,
                Infrastructure.Services.VehicleVerificationService>();
        builder.Services.ConfigureCorsPolicy();
        builder.Services.AddMemoryCache();

        WebApplication app = builder.Build();

        await using AsyncServiceScope serviceScope = app.Services.CreateAsyncScope();
        await using IDataContext dataContext = serviceScope.ServiceProvider.GetRequiredService<IDataContext>();
        await dataContext.Database.EnsureCreatedAsync();

        await app.Services.SeedIdentity();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwaggerDocumentation();
        }

        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseErrorHandler();
        app.MapControllers();

        app.MapHub<ChatHub>("/hubs/chat");

        await app.RunAsync();
    }
}