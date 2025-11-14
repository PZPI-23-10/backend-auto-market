using Api.Extensions;
using Application.Extensions;
using Application.Validation;
using FluentValidation;
using Infrastructure.Extensions;
using Infrastructure.Persistence;

namespace Api;

public static class Program
{
    public static void Main(string[] args)
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

        WebApplication app = builder.Build();

        IServiceScope serviceScope = app.Services.CreateScope();
        DataContext dataContext = serviceScope.ServiceProvider.GetRequiredService<DataContext>();
        dataContext.Database.EnsureCreated();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwaggerDocumentation();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseErrorHandler();
        app.MapControllers();

        app.Run();
    }
}