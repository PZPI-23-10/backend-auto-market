using Api.Extensions;
using Application.Extensions;
using Application.Validation;
using FluentValidation;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Application.Interfaces.Services;
using Application.Services;

namespace Api;

public static class Program
{
    public static async Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddJwtAuthentication(builder.Configuration);
        builder.Services.AddAuthorization();

        builder.Services.AddApiControllers();

        builder.Services.AddValidatorsFromAssembly(typeof(RegisterUserRequestValidator).Assembly);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerDocumentation();

        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddApplicationServices();

        builder.Services.AddPersistence(builder.Configuration);

        builder.Services.ConfigureCorsPolicy();
        builder.Services.AddMemoryCache();

        builder.Services.AddScoped<IListingService, ListingService>();
        
        WebApplication app = builder.Build();

        await using AsyncServiceScope serviceScope = app.Services.CreateAsyncScope();
        await using DataContext dataContext = serviceScope.ServiceProvider.GetRequiredService<DataContext>();
        await dataContext.Database.EnsureCreatedAsync();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwaggerDocumentation();
        }

        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseErrorHandler();
        app.MapControllers();

        app.Run();
    }
}