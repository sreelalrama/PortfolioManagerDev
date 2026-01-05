using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using StockTradePro.UserManagement.API.Models;
using StockTradePro.UserManagement.API.Models.DTOs;
using StockTradePro.UserManagement.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockTradePro.UserManagement.UnitTests.Services
{
    public class UserServiceTests
    {
        //create mock instance
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<SignInManager<ApplicationUser>> _mockSignInManager;

        private readonly Mock<ITokenService> _mockTokenService;
        private readonly Mock<ILogger<UserService>> _mockLogger;


        private readonly UserService _userService;



        public UserServiceTests()
        {
            _mockUserManager = MockUserManager();
            _mockTokenService = new Mock<ITokenService>();
            _mockSignInManager = MockSignInManager(_mockUserManager.Object);
            _mockLogger = new Mock<ILogger<UserService>>();

            _userService = new UserService(
                _mockUserManager.Object,
                _mockSignInManager.Object,
                _mockTokenService.Object,
                _mockLogger.Object
            );

        }

        

        [Fact]
        public async Task RegisterAsync_ShouldReturnSuccess_WhenUserDoesNotExist()
        {
            //arrange
            var registrationDto = new UserRegistrationDto
            {
                Email = "test@StockTradePro.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                FirstName = "Test",
                LastName = "User"
            };

            _mockUserManager.Setup(x=> x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(null as ApplicationUser);

            _mockUserManager.Setup(x=> x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())) .ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(x=> x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<string>() { "User" });

            _mockTokenService.Setup(x => x.GenerateJwtTokenAsync(It.IsAny<ApplicationUser>()))
               .ReturnsAsync("valid-token");

            _mockTokenService.Setup(x => x.GenerateRefreshTokenAsync())
                .ReturnsAsync("valid-refresh-token");

            //act
            var result = await _userService.RegisterAsync(registrationDto);

            //assert
            Assert.True(result.IsSuccess);
            Assert.Equal("valid-token", result.Token);
            Assert.Equal("valid-refresh-token", result.RefreshToken);
            Assert.NotNull(result.User);
            Assert.Equal("test@StockTradePro.com", result.User.Email);



        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnFailure_WhenUserAlreadyExists()
        {
            // arrange
            var registrationDto = new UserRegistrationDto { Email = "existing@StockTradePro.com" };

            _mockUserManager.Setup(x => x.FindByEmailAsync(registrationDto.Email))
                .ReturnsAsync(new ApplicationUser { Email = registrationDto.Email });

            // act
            var result = await _userService.RegisterAsync(registrationDto);

            // assert
            Assert.False(result.IsSuccess);
            Assert.Contains("already exists", result.Message);
            _mockUserManager.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnSuccess_WhenCredentialsAreValid()
        {
            // arrange
            var loginDto = new UserLoginDto { Email = "test@StockTradePro.com", Password = "Password123!" };
            var user = new ApplicationUser { Id = "1", Email = loginDto.Email, IsActive = true };

            _mockUserManager.Setup(x => x.FindByEmailAsync(loginDto.Email))
                .ReturnsAsync(user);

            _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(user, loginDto.Password, false))
                .ReturnsAsync(SignInResult.Success);

            _mockUserManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "User" });

            _mockTokenService.Setup(x => x.GenerateJwtTokenAsync(user))
                .ReturnsAsync("valid-token");

            // act
            var result = await _userService.LoginAsync(loginDto);

            // assert
            Assert.True(result.IsSuccess);
            Assert.Equal("valid-token", result.Token);
            _mockUserManager.Verify(x => x.UpdateAsync(user), Times.Once); // Should update LastLoginAt
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnFailure_WhenUserNotFound()
        {
            // Arrange
            var loginDto = new UserLoginDto { Email = "nonexistent@StockTradePro.com", Password = "Password123!" };

            _mockUserManager.Setup(x => x.FindByEmailAsync(loginDto.Email))
                .ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _userService.LoginAsync(loginDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Invalid email", result.Message);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnFailure_WhenPasswordIsInvalid()
        {
            // arrange
            var loginDto = new UserLoginDto { Email = "test@StockTradePro.com", Password = "WrongPassword" };
            var user = new ApplicationUser { Email = loginDto.Email, IsActive = true };

            _mockUserManager.Setup(x => x.FindByEmailAsync(loginDto.Email))
                .ReturnsAsync(user);

            _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(user, loginDto.Password, false))
                .ReturnsAsync(SignInResult.Failed);

            // Act
            var result = await _userService.LoginAsync(loginDto);

            // assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Invalid email", result.Message);
        }


        private static Mock<UserManager<ApplicationUser>> MockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);
        }

        private static Mock<SignInManager<ApplicationUser>> MockSignInManager(UserManager<ApplicationUser> userManager)
        {
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var options = new Mock<IOptions<IdentityOptions>>();
            var logger = new Mock<ILogger<SignInManager<ApplicationUser>>>();
            var schemes = new Mock<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>();
            var confirmation = new Mock<IUserConfirmation<ApplicationUser>>();

            return new Mock<SignInManager<ApplicationUser>>(
                userManager,
                contextAccessor.Object,
                claimsFactory.Object,
                options.Object,
                logger.Object,
                schemes.Object,
                confirmation.Object);
        }
    }
}
