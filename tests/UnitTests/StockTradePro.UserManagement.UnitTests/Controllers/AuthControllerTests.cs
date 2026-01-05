using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using StockTradePro.UserManagement.API.Controllers;
using StockTradePro.UserManagement.API.Models.DTOs;
using StockTradePro.UserManagement.API.Services;

namespace StockTradePro.UserManagement.UnitTests.Controllers
{
    public  class AuthControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<ILogger<AuthController>> _mockLogger;
        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger<AuthController>>();
            _authController = new AuthController(_mockUserService.Object, _mockLogger.Object);

        }

        [Fact]
        public async Task Register_ShouldReturnOk_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var dto = new UserRegistrationDto { Email = "test@test.com", Password = "Pass" };
            var response = new AuthResponseDto { IsSuccess = true, Token = "token" };

            _mockUserService.Setup(x => x.RegisterAsync(dto))
                .ReturnsAsync(response);

            // Act
            var result = await _authController.Register(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnDto = Assert.IsType<AuthResponseDto>(okResult.Value);
            Assert.True(returnDto.IsSuccess);
        }

        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenRegistrationFails()
        {
            // Arrange
            var dto = new UserRegistrationDto { Email = "pool@test.com", Password = "Pass" };
            var response = new AuthResponseDto { IsSuccess = false, Message = "Email exists" };

            _mockUserService.Setup(x => x.RegisterAsync(dto))
                .ReturnsAsync(response);

            // Act
            var result = await _authController.Register(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var returnDto = Assert.IsType<AuthResponseDto>(badRequestResult.Value);
            Assert.False(returnDto.IsSuccess);
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenLoginIsSuccessful()
        {
            // Arrange
            var dto = new UserLoginDto { Email = "test@test.com", Password = "Pass" };
            var response = new AuthResponseDto { IsSuccess = true };

            _mockUserService.Setup(x => x.LoginAsync(dto))
                .ReturnsAsync(response);

            // Act
            var result = await _authController.Login(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenLoginFails()
        {
            // Arrange
            var dto = new UserLoginDto { Email = "test@test.com", Password = "Wrong" };
            var response = new AuthResponseDto { IsSuccess = false };

            _mockUserService.Setup(x => x.LoginAsync(dto))
                .ReturnsAsync(response);

            // Act
            var result = await _authController.Login(dto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }
    }
}
