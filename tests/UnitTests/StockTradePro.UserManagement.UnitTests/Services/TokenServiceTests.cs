using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using StackExchange.Redis;
using StockTradePro.UserManagement.API.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace StockTradePro.UserManagement.UnitTests.Services
{
    public class TokenServiceTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IConnectionMultiplexer> _mockRedis;
        private readonly Mock<IDatabase> _mockDatabase;
        private readonly TokenService _tokenService;

        public TokenServiceTests()
        {
            _mockUserManager = MockUserManager();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockRedis = new Mock<IConnectionMultiplexer>();
            _mockDatabase = new Mock<IDatabase>();

            // Setup Redis mock to return our mocked database
            _mockRedis.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Returns(_mockDatabase.Object);

            // Setup Configuration mocks
            _mockConfiguration.Setup(x => x["Jwt:SecretKey"]).Returns("SuperSecretKeyForTestingPurposesOnly12345!");
            _mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns("StockTradePro");
            _mockConfiguration.Setup(x => x["Jwt:Audience"]).Returns("StockTradeProUser");
            _mockConfiguration.Setup(x => x["Jwt:ExpirationMinutes"]).Returns("60");
            _mockConfiguration.Setup(x => x["Jwt:RefreshTokenExpirationDays"]).Returns("7");

            _tokenService = new TokenService(
                _mockUserManager.Object,
                _mockConfiguration.Object,
                _mockRedis.Object
            );
        }

        [Fact]
        public async Task GenerateJwtTokenAsync_ShouldReturnValidToken_WhenUserIsValid()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = "user-1",
                Email = "test@StockTradePro.com",
                FirstName = "Test",
                LastName = "User"
            };

            _mockUserManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "User", "Admin" });

            // Act
            var tokenString = await _tokenService.GenerateJwtTokenAsync(user);

            // Assert
            Assert.NotNull(tokenString);
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenString);

            Assert.Equal("StockTradePro", token.Issuer);
            Assert.Equal("StockTradeProUser", token.Audiences.First());

            // Verify Claims
            Assert.Contains(token.Claims, c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == "user-1");
            Assert.Contains(token.Claims, c => c.Type == JwtRegisteredClaimNames.Email && c.Value == "test@StockTradePro.com");
            Assert.Contains(token.Claims, c => c.Type == "first_name" && c.Value == "Test");
            Assert.Contains(token.Claims, c => c.Type == ClaimTypes.Role && c.Value == "User");
            Assert.Contains(token.Claims, c => c.Type == ClaimTypes.Role && c.Value == "Admin");
        }

        [Fact]
        public async Task GenerateRefreshTokenAsync_ShouldReturnRandomString()
        {
            // act
            var refreshToken1 = await _tokenService.GenerateRefreshTokenAsync();
            var refreshToken2 = await _tokenService.GenerateRefreshTokenAsync();

            // Assert
            Assert.NotNull(refreshToken1);
            Assert.NotNull(refreshToken2);
            Assert.NotEqual(refreshToken1, refreshToken2); // Should be random
            Assert.True(refreshToken1.Length > 20); // Should be reasonably long base64 string
        }

        [Fact]
        public async Task SaveRefreshTokenAsync_ShouldStoreInRedis()
        {
            // Arrange
            var userId = "user-1";
            var refreshToken = "some-refresh-token";

            // Act
            await _tokenService.SaveRefreshTokenAsync(userId, refreshToken);

            // Assert
            _mockDatabase.Verify(x => x.StringSetAsync(
                $"refresh_token:{userId}",
                refreshToken,
                It.IsAny<TimeSpan?>(),
                false,
                When.Always,
                CommandFlags.None), Times.Once);
        }

        [Fact]
        public async Task ValidateRefreshTokenAsync_ShouldReturnTrue_WhenTokenMatches()
        {
            // Arrange
            var userId = "user-1";
            var refreshToken = "valid-refresh-token";

            _mockDatabase.Setup(x => x.StringGetAsync($"refresh_token:{userId}", CommandFlags.None))
                .ReturnsAsync(refreshToken);

            // Act
            var result = await _tokenService.ValidateRefreshTokenAsync(userId, refreshToken);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ValidateRefreshTokenAsync_ShouldReturnFalse_WhenTokenDoesNotMatch()
        {
            // Arrange
            var userId = "user-1";
            var storedToken = "actual-stored-token";
            var providedToken = "different-token";

            _mockDatabase.Setup(x => x.StringGetAsync($"refresh_token:{userId}", CommandFlags.None))
                .ReturnsAsync(storedToken);

            // Act
            var result = await _tokenService.ValidateRefreshTokenAsync(userId, providedToken);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ValidateRefreshTokenAsync_ShouldReturnFalse_WhenTokenDoesNotExist()
        {
            // Arrange
            var userId = "user-1";

            _mockDatabase.Setup(x => x.StringGetAsync($"refresh_token:{userId}", CommandFlags.None))
                .ReturnsAsync(RedisValue.Null);

            // Act
            var result = await _tokenService.ValidateRefreshTokenAsync(userId, "any-token");

            // Assert
            Assert.False(result);

        }

        [Fact]
        public async Task RevokeRefreshTokenAsync_ShouldDeleteFromRedis()
        {
            // Arrange
            var userId = "user-1";

            // Act
            await _tokenService.RevokeRefreshTokenAsync(userId);

            // Assert
            _mockDatabase.Verify(x => x.KeyDeleteAsync(
                $"refresh_token:{userId}",
                CommandFlags.None), Times.Once);
        }

        private static Mock<UserManager<ApplicationUser>> MockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);
        }
    }
}
