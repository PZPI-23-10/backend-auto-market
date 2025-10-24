using System.Text;
using System.Text.Json.Serialization;
using backend_auto_market.Extensions;
using backend_auto_market.Persistence;
using backend_auto_market.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace backend_auto_market;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthentication(cfg =>
        {
            cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            cfg.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = false;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8
                        .GetBytes(builder.Configuration.GetSection("JwtSettings")["AccessSecretKey"]!)
                ),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };
        });

        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });

            c.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri("https://accounts.google.com/o/oauth2/auth"),
                        TokenUrl = new Uri("https://oauth2.googleapis.com/token"),
                        Scopes = new Dictionary<string, string>
                        {
                            { "openid", "OpenID" },
                            { "email", "Email" },
                            { "profile", "Profile" }
                        }
                    }
                }
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "OAuth2" }
                    },
                    new[] { "openid", "email", "profile" }
                }
            });
        });


        // builder.Services.AddCors(options =>
        // {
        //     options.AddPolicy("AllowSwagger", policy =>
        //     {
        //         policy.WithOrigins("http://localhost:5291")
        //             .AllowAnyHeader()
        //             .AllowAnyMethod();
        //     });
        //
        //     var allowedOrigins = new[]
        //     {
        //         "http://localhost:5291",
        //         "https://your-production-client.com"
        //     };
        //
        //     options.AddPolicy("CorsPolicy", builder =>
        //     {
        //         builder
        //             .WithOrigins(allowedOrigins)
        //             .AllowAnyHeader()
        //             .AllowAnyMethod()
        //             .AllowCredentials();
        //     });
        // });

        builder.Services.ConfigureDatabase(builder.Configuration);
        builder.Services.AddAuthorization();
        builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
        builder.Services.AddScoped<TokenService>();

        var app = builder.Build();

        var serviceScope = app.Services.CreateScope();
        var dataContext = serviceScope.ServiceProvider.GetRequiredService<DataContext>();
        dataContext.Database.EnsureCreated();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        
        app.UseCors("AllowSwagger");
        app.UseCors("AllowSpecificOrigins");

        app.UseAuthorization();
        app.UseAuthentication();
        app.UseErrorHandler();
        app.MapControllers();

        app.Run();
    }
}