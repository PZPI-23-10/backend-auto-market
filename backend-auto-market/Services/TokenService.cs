using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace backend_auto_market.Services;

public class TokenService
{
    private readonly string _accessSecretKey;
    private readonly string _refreshSecretKey;
    private readonly int _accessExpirationTimeInMinutes;
    private readonly int _rememberAccessExpirationTimeInDays;
    private readonly int _refreshExpirationTimeInHours;

    public TokenService(IConfiguration configuration)
    {
        _accessSecretKey = configuration.GetSection("JwtSettings")["AccessSecretKey"]!;
        _refreshSecretKey = configuration.GetSection("JwtSettings")["RefreshSecretKey"]!;
        _accessExpirationTimeInMinutes =
            int.Parse(configuration.GetSection("JwtSettings")["DefaultAccessExpireTimeInMinutes"]!);

        _rememberAccessExpirationTimeInDays =
            int.Parse(configuration.GetSection("JwtSettings")["RememberAccessExpireTimeInDays"]!);

        _refreshExpirationTimeInHours =
            int.Parse(configuration.GetSection("JwtSettings")["RefreshExpireTimeInMinutes"]!);
    }

    public DateTime GetTokenExpirationTime(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var decodedToken = tokenHandler.ReadToken(token);
        return decodedToken.ValidTo;
    }

    public Token GenerateAccessToken(string id, string email, bool rememberMe)
    {
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, id), new(ClaimTypes.Name, email) };
        var expirationTime = rememberMe
            ? DateTime.UtcNow.AddDays(_rememberAccessExpirationTimeInDays)
            : DateTime.UtcNow.AddMinutes(_accessExpirationTimeInMinutes);

        return GenerateToken(_accessSecretKey, expirationTime, claims);
    }

    public ClaimsPrincipal ValidateAccessToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_accessSecretKey);

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out _);

            return principal;
        }
        catch
        {
            return null;
        }
    }

    private Token GenerateToken(string secretKey, DateTime expirationTime, List<Claim> claims)
    {
        var jwtToken = new JwtSecurityToken(
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expirationTime,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(secretKey)
                ),
                SecurityAlgorithms.HmacSha256Signature));

        return new Token { TokenKey = new JwtSecurityTokenHandler().WriteToken(jwtToken), Expires = expirationTime };
    }
}