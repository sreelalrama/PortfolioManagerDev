using StockTradePro.BlazorUI.Models.Auth;
using StockTradePro.BlazorUI.Services;

namespace StockTradePro.BlazorUI.Services.Mocks
{
    public class MockAuthService : IAuthService
    {
        private string? _currentToken;
        private UserDto? _currentUser;
        private readonly List<UserDto> _users;

        public MockAuthService()
        {
            // Initialize with some mock users
            _users = new List<UserDto>
            {
                new UserDto
                {
                    Id = "1",
                    Email = "john.doe@example.com",
                    FirstName = "John",
                    LastName = "Doe",
                    PhoneNumber = "+1234567890",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    LastLoginAt = DateTime.UtcNow.AddHours(-2),
                    IsActive = true,
                    DateOfBirth = new DateTime(1990, 5, 15),
                    Bio = "Software Developer",
                    Roles = new List<string> { "User", "Premium" }
                },
                new UserDto
                {
                    Id = "2",
                    Email = "jane.smith@example.com",
                    FirstName = "Jane",
                    LastName = "Smith",
                    PhoneNumber = "+1987654321",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-15),
                    LastLoginAt = DateTime.UtcNow.AddDays(-1),
                    IsActive = true,
                    DateOfBirth = new DateTime(1985, 8, 22),
                    Bio = "Financial Analyst",
                    Roles = new List<string> { "User" }
                }
            };
        }

        public async Task<AuthResponseDto?> LoginAsync(UserLoginDto loginDto)
        {
            await Task.Delay(500); // Simulate API delay

            // Mock login logic - accept any email with password "password"
            var user = _users.FirstOrDefault(u =>
                u.Email.Equals(loginDto.Email, StringComparison.OrdinalIgnoreCase));

            if (user != null && loginDto.Password == "password")
            {
                _currentUser = user;
                _currentUser.LastLoginAt = DateTime.UtcNow;
                _currentToken = $"mock_token_{Guid.NewGuid()}";

                return new AuthResponseDto
                {
                    IsSuccess = true,
                    Token = _currentToken,
                    RefreshToken = $"refresh_token_{Guid.NewGuid()}",
                    ExpiresAt = DateTime.UtcNow.AddHours(1),
                    User = user,
                    Message = "Login successful"
                };
            }

            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = "Invalid email or password"
            };
        }

        public async Task<AuthResponseDto?> RegisterAsync(UserRegistrationDto registrationDto)
        {
            await Task.Delay(500); // Simulate API delay

            // Validate passwords match
            if (registrationDto.Password != registrationDto.ConfirmPassword)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Passwords do not match"
                };
            }

            // Check if user already exists
            if (_users.Any(u => u.Email.Equals(registrationDto.Email, StringComparison.OrdinalIgnoreCase)))
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Email already exists"
                };
            }

            // Create new user
            var newUser = new UserDto
            {
                Id = (_users.Count + 1).ToString(),
                Email = registrationDto.Email,
                FirstName = registrationDto.FirstName,
                LastName = registrationDto.LastName,
                PhoneNumber = registrationDto.PhoneNumber,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow,
                IsActive = true,
                Roles = new List<string> { "User" }
            };

            _users.Add(newUser);
            _currentUser = newUser;
            _currentToken = $"mock_token_{Guid.NewGuid()}";

            return new AuthResponseDto
            {
                IsSuccess = true,
                Token = _currentToken,
                RefreshToken = $"refresh_token_{Guid.NewGuid()}",
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                User = newUser,
                Message = "Registration successful"
            };
        }

        public async Task<AuthResponseDto?> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
        {
            await Task.Delay(200); // Simulate API delay

            if (_currentUser != null && !string.IsNullOrEmpty(refreshTokenDto.RefreshToken) &&
                _currentUser.Id == refreshTokenDto.UserId)
            {
                _currentToken = $"mock_token_{Guid.NewGuid()}";

                return new AuthResponseDto
                {
                    IsSuccess = true,
                    Token = _currentToken,
                    RefreshToken = $"refresh_token_{Guid.NewGuid()}",
                    ExpiresAt = DateTime.UtcNow.AddHours(1),
                    User = _currentUser,
                    Message = "Token refreshed successfully"
                };
            }

            return new AuthResponseDto
            {
                IsSuccess = false,
                Message = "Invalid refresh token"
            };
        }

        public async Task<bool> LogoutAsync()
        {
            await Task.Delay(200); // Simulate API delay

            _currentToken = null;
            _currentUser = null;
            return true;
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            await Task.Delay(500); // Simulate API delay

            // Mock: return true if user exists
            return _users.Any(u => u.Email.Equals(forgotPasswordDto.Email, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            await Task.Delay(500); // Simulate API delay

            // Mock: always return true for valid token format
            return !string.IsNullOrEmpty(resetPasswordDto.Token) &&
                   !string.IsNullOrEmpty(resetPasswordDto.NewPassword);
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            await Task.Delay(300); // Simulate API delay

            // Validate passwords match
            if (changePasswordDto.NewPassword != changePasswordDto.ConfirmNewPassword)
            {
                return false;
            }

            // Mock: return true if current password is "password"
            return _currentUser != null && changePasswordDto.CurrentPassword == "password";
        }

        public async Task<UserDto?> GetCurrentUserAsync()
        {
            await Task.Delay(100); // Simulate API delay
            return _currentUser;
        }

        public async Task<UserDto?> GetUserAsync(string userId)
        {
            await Task.Delay(200); // Simulate API delay
            return _users.FirstOrDefault(u => u.Id == userId);
        }

        public async Task<bool> UpdateProfileAsync(UserDto userDto)
        {
            await Task.Delay(300); // Simulate API delay

            if (_currentUser != null && _currentUser.Id == userDto.Id)
            {
                // Update current user
                _currentUser.FirstName = userDto.FirstName;
                _currentUser.LastName = userDto.LastName;
                _currentUser.Email = userDto.Email;
                _currentUser.PhoneNumber = userDto.PhoneNumber;
                _currentUser.DateOfBirth = userDto.DateOfBirth;
                _currentUser.Bio = userDto.Bio;
                _currentUser.ProfileImageUrl = userDto.ProfileImageUrl;

                // Update in users list
                var userInList = _users.FirstOrDefault(u => u.Id == userDto.Id);
                if (userInList != null)
                {
                    userInList.FirstName = userDto.FirstName;
                    userInList.LastName = userDto.LastName;
                    userInList.Email = userDto.Email;
                    userInList.PhoneNumber = userDto.PhoneNumber;
                    userInList.DateOfBirth = userDto.DateOfBirth;
                    userInList.Bio = userDto.Bio;
                    userInList.ProfileImageUrl = userDto.ProfileImageUrl;
                }

                return true;
            }

            return false;
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            await Task.Delay(50); // Simulate minimal delay
            return !string.IsNullOrEmpty(_currentToken) && _currentUser != null;
        }

        public async Task<string?> GetCurrentUserIdAsync()
        {
            await Task.Delay(50); // Simulate minimal delay
            return _currentUser?.Id;
        }

        public async Task<string?> GetTokenAsync()
        {
            await Task.Delay(50); // Simulate minimal delay
            return _currentToken;
        }

        public void SetToken(string token)
        {
            _currentToken = token;

            // Mock: if token is set, assume user "1" is logged in
            if (!string.IsNullOrEmpty(token))
            {
                _currentUser = _users.FirstOrDefault(u => u.Id == "1");
            }
        }

        public void ClearToken()
        {
            _currentToken = null;
            _currentUser = null;
        }
    }
}