using Microsoft.VisualStudio.TestPlatform.TestHost;
using StockTradePro.UserManagement.API.Models.DTOs;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace StockTradePro.UserManagement.IntegrationTests
{
    public class AuthIntegrationTests
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public AuthIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Register_And_Login_Flow_ShouldSuccess()
        {
            // 1. Register
            var registerDto = new UserRegistrationDto
            {
                Email = "integration@test.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                FirstName = "Integration",
                LastName = "Tester"
            };

            var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerDto);

            // Assert Registration
            registerResponse.EnsureSuccessStatusCode();
            var registerData = await registerResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
            Assert.True(registerData?.IsSuccess);
            Assert.NotNull(registerData?.Token);


            // 2. Login with same credentials
            var loginDto = new UserLoginDto
            {
                Email = "integration@test.com",
                Password = "Password123!"
            };

            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

            // Assert Login
            loginResponse.EnsureSuccessStatusCode();
            var loginData = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();
            Assert.True(loginData?.IsSuccess);
            Assert.Equal(registerData?.User.Email, loginData?.User.Email);
        }

        [Fact]
        public async Task Login_WithInvalidPassword_ShouldReturnUnauthorized()
        {
            // 1. Register a user
            var registerDto = new UserRegistrationDto
            {
                Email = "fail@test.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                FirstName = "Fail",
                LastName = "User"
            };
            await _client.PostAsJsonAsync("/api/auth/register", registerDto);

            // 2. Try Login with wrong password
            var loginDto = new UserLoginDto
            {
                Email = "fail@test.com",
                Password = "WrongPassword!"
            };

            var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
