using StockTradePro.UserManagement.API.Models.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockTradePro.UserManagement.UnitTests.Domain
{
    public class DtoValidationTests
    {

        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }


        [Fact]
        public void UserRegistrationDto_ShouldHaveErrors_WhenRequiredFieldsAreMissing()
        {
            // Arrange
            var dto = new UserRegistrationDto(); // Empty

            // Act
            var errors = ValidateModel(dto);

            // Assert
            // Expecting errors for: Email, Password, FirstName, LastName
            Assert.NotEmpty(errors);
            Assert.Contains(errors, v => v.MemberNames.Contains("Email"));
            Assert.Contains(errors, v => v.MemberNames.Contains("Password"));
            Assert.Contains(errors, v => v.MemberNames.Contains("FirstName"));
            Assert.Contains(errors, v => v.MemberNames.Contains("LastName"));
        }

        [Fact]
        public void UserRegistrationDto_ShouldHaveError_WhenEmailIsInvalid()
        {
            // arrange
            var dto = new UserRegistrationDto
            {
                Email = "invalid-email",
                Password = "Pass",
                ConfirmPassword = "Pass",
                FirstName = "Test",
                LastName = "User"
            };

            // Act
            var errors = ValidateModel(dto);

            // Assert
            Assert.Contains(errors, v => v.MemberNames.Contains("Email"));
        }

        [Fact]
        public void UserRegistrationDto_ShouldHaveError_WhenPasswordsDoNotMatch()
        {
            // Arrange
            var dto = new UserRegistrationDto
            {
                Email = "valid@test.com",
                Password = "Password123!",
                ConfirmPassword = "DifferentPassword",
                FirstName = "Test",
                LastName = "User"
            };

            // Act
            var errors = ValidateModel(dto);

            // Assert
            Assert.Contains(errors, v => v.MemberNames.Contains("ConfirmPassword"));
            Assert.Contains(errors, v => v.ErrorMessage!.Contains("match"));
        }

        [Fact]
        public void UserLoginDto_ShouldHaveErrors_WhenFieldsAreEmpty()
        {
            // Arrange
            var dto = new UserLoginDto();

            // Act
            var errors = ValidateModel(dto);

            // Assert
            Assert.Contains(errors, v => v.MemberNames.Contains("Email"));
            Assert.Contains(errors, v => v.MemberNames.Contains("Password"));
        }
    }
}
