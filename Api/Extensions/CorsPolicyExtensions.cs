namespace Api.Extensions;

public static class CorsPolicyExtensions
{
    public static IServiceCollection ConfigureCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder => builder
                .WithOrigins(
                    "http://localhost:5173"
                )
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
            );
        });

        return services;
    }
}