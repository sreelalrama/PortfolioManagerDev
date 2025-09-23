using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using StockTradePro.UserManagement.API.Models;
using StockTradePro.UserManagement.API.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public class TokenService : ITokenService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IDatabase _redisDb;

    public TokenService(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        IConnectionMultiplexer redis)
    {
        _userManager = userManager;
        _configuration = configuration;
        _redisDb = redis.GetDatabase();
    }

    public async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
    {
        var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim("first_name", user.FirstName ?? string.Empty),
                new Claim("last_name", user.LastName ?? string.Empty)
            };

        // Add roles to claims
        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"] ?? ""));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpirationMinutes"])),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<string> GenerateRefreshTokenAsync()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public async Task<bool> ValidateRefreshTokenAsync(string userId, string refreshToken)
    {
        var storedToken = await _redisDb.StringGetAsync($"refresh_token:{userId}");
        return storedToken.HasValue && storedToken == refreshToken;
    }

    public async Task SaveRefreshTokenAsync(string userId, string refreshToken)
    {
        var expiry = TimeSpan.FromDays(Convert.ToDouble(_configuration["Jwt:RefreshTokenExpirationDays"]));
        await _redisDb.StringSetAsync($"refresh_token:{userId}", refreshToken, expiry);
    }

    public async Task RevokeRefreshTokenAsync(string userId)
    {
        await _redisDb.KeyDeleteAsync($"refresh_token:{userId}");
    }
}