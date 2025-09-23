using Microsoft.AspNetCore.Identity;
using StockTradePro.UserManagement.API.Models;
using StockTradePro.UserManagement.API.Models.DTOs;

namespace StockTradePro.UserManagement.API.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly ILogger<UserService> _logger;

        public UserService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            ILogger<UserService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<AuthResponseDto> RegisterAsync(UserRegistrationDto registrationDto)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(registrationDto.Email);
                if (existingUser != null)
                {
                    return new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "User with this email already exists."
                    };
                }

                var user = new ApplicationUser
                {
                    UserName = registrationDto.Email,
                    Email = registrationDto.Email,
                    FirstName = registrationDto.FirstName,
                    LastName = registrationDto.LastName,
                    PhoneNumber = registrationDto.PhoneNumber,
                    EmailConfirmed = false // Will be confirmed via email
                };

                var result = await _userManager.CreateAsync(user, registrationDto.Password);
                if (!result.Succeeded)
                {
                    return new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = string.Join(", ", result.Errors.Select(e => e.Description))
                    };
                }

                // Add default role
                await _userManager.AddToRoleAsync(user, "User");

                // Generate tokens
                var token = await _tokenService.GenerateJwtTokenAsync(user);
                var refreshToken = await _tokenService.GenerateRefreshTokenAsync();
                await _tokenService.SaveRefreshTokenAsync(user.Id, refreshToken);

                var userDto = await MapToUserDtoAsync(user);

                return new AuthResponseDto
                {
                    IsSuccess = true,
                    Token = token,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60), // Match JWT expiration
                    User = userDto,
                    Message = "Registration successful."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration");
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "An error occurred during registration."
                };
            }
        }

        public async Task<AuthResponseDto> LoginAsync(UserLoginDto loginDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                if (user == null || !user.IsActive)
                {
                    return new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "Invalid email or password."
                    };
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
                if (!result.Succeeded)
                {
                    return new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "Invalid email or password."
                    };
                }

                // Update last login
                user.LastLoginAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                // Generate tokens
                var token = await _tokenService.GenerateJwtTokenAsync(user);
                var refreshToken = await _tokenService.GenerateRefreshTokenAsync();
                await _tokenService.SaveRefreshTokenAsync(user.Id, refreshToken);

                var userDto = await MapToUserDtoAsync(user);

                return new AuthResponseDto
                {
                    IsSuccess = true,
                    Token = token,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                    User = userDto,
                    Message = "Login successful."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user login");
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "An error occurred during login."
                };
            }
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken, string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null || !user.IsActive)
                {
                    return new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "User not found or inactive."
                    };
                }

                var isValidRefreshToken = await _tokenService.ValidateRefreshTokenAsync(userId, refreshToken);
                if (!isValidRefreshToken)
                {
                    return new AuthResponseDto
                    {
                        IsSuccess = false,
                        Message = "Invalid refresh token."
                    };
                }

                // Generate new tokens
                var newToken = await _tokenService.GenerateJwtTokenAsync(user);
                var newRefreshToken = await _tokenService.GenerateRefreshTokenAsync();
                await _tokenService.SaveRefreshTokenAsync(user.Id, newRefreshToken);

                var userDto = await MapToUserDtoAsync(user);

                return new AuthResponseDto
                {
                    IsSuccess = true,
                    Token = newToken,
                    RefreshToken = newRefreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                    User = userDto,
                    Message = "Token refreshed successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "An error occurred during token refresh."
                };
            }
        }

        public async Task<bool> LogoutAsync(string userId)
        {
            try
            {
                await _tokenService.RevokeRefreshTokenAsync(userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return false;
            }
        }

        public async Task<UserDto?> GetUserByIdAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                return user != null ? await MapToUserDtoAsync(user) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID: {UserId}", userId);
                return null;
            }
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                return user != null ? await MapToUserDtoAsync(user) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by email: {Email}", email);
                return null;
            }
        }

        public async Task<bool> UpdateUserAsync(string userId, UserDto userDto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return false;

                user.FirstName = userDto.FirstName;
                user.LastName = userDto.LastName;
                user.PhoneNumber = userDto.PhoneNumber;
                user.ProfileImageUrl = userDto.ProfileImageUrl;
                user.DateOfBirth = userDto.DateOfBirth;
                user.Bio = userDto.Bio;

                var result = await _userManager.UpdateAsync(user);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user: {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return false;

                var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user: {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null) return false; // Don't reveal if email exists

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                // TODO: Send email with reset token
                // For now, just log it (in production, integrate with email service)
                _logger.LogInformation("Password reset token for {Email}: {Token}", email, token);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password reset request for email: {Email}", email);
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null) return false;

                var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password reset for email: {Email}", email);
                return false;
            }
        }

        private async Task<UserDto> MapToUserDtoAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                IsActive = user.IsActive,
                ProfileImageUrl = user.ProfileImageUrl,
                DateOfBirth = user.DateOfBirth,
                Bio = user.Bio,
                Roles = roles.ToList()
            };
        }
    }
}
