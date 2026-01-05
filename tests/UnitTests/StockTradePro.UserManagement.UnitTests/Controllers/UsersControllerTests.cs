using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using StockTradePro.UserManagement.API.Controllers;
using StockTradePro.UserManagement.API.Models.DTOs;
using StockTradePro.UserManagement.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace StockTradePro.UserManagement.UnitTests.Controllers
{
    public class UsersControllerTests
    {


        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<ILogger<UsersController>> _mockLogger;
        private readonly UsersController _usersController;


        public UsersControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger<UsersController>>();
            _usersController = new UsersController(_mockUserService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetProfile_ShouldReturnOk_WhenUserExists()
        {
            // Arrange
            var userId = "123";
            SetupHttpContext(userId);

            var userDto = new UserDto { Id = userId, Email = "test@test.com" };
            _mockUserService.Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(userDto);

            // Act
            var result = await _usersController.GetProfile();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedDto = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal(userId, returnedDto.Id);
        }

        [Fact]
        public async Task GetProfile_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "123";
            SetupHttpContext(userId);

            _mockUserService.Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync((UserDto?)null);

            // Act
            var result = await _usersController.GetProfile();

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);

        }

        [Fact]
        public async Task UpdateProfile_ShouldReturnOk_WhenUpdateSucceeds()
        {
            // arrange
            var userId = "123";
            SetupHttpContext(userId);
            var updateDto = new UserDto { FirstName = "New" };

            _mockUserService.Setup(x => x.UpdateUserAsync(userId, updateDto))
                .ReturnsAsync(true);

            // Act
            var result = await _usersController.UpdateProfile(updateDto);

            // assert
            var okResult = Assert.IsType<OkObjectResult>(result);

        }

        private void SetupHttpContext(string userId)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }));

            _usersController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }
    }
}
