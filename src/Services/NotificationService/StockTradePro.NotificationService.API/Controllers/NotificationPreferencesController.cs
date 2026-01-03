using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using StockTradePro.NotificationService.API.DTOs;
using StockTradePro.NotificationService.API.Models;
using StockTradePro.NotificationService.API.Services;

namespace StockTradePro.NotificationService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationPreferencesController : ControllerBase
    {
        private readonly IUserNotificationPreferenceService _preferenceService;
        private readonly ILogger<NotificationPreferencesController> _logger;

        public NotificationPreferencesController(
            IUserNotificationPreferenceService preferenceService,
            ILogger<NotificationPreferencesController> logger)
        {
            _preferenceService = preferenceService;
            _logger = logger;
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                   User.FindFirst("sub")?.Value ??
                   User.FindFirst("userId")?.Value ??
                   "anonymous"; // For testing without authentication
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationPreferenceDto>>> GetPreferences()
        {
            try
            {
                var userId = GetUserId();
                var preferences = await _preferenceService.GetUserPreferencesAsync(userId);
                return Ok(preferences);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notification preferences");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{type}")]
        public async Task<ActionResult<NotificationPreferenceDto>> GetPreference(NotificationType type)
        {
            try
            {
                var userId = GetUserId();
                var preference = await _preferenceService.GetUserPreferenceAsync(userId, type);

                if (preference == null)
                    return NotFound($"Preference for notification type {type} not found");

                return Ok(preference);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notification preference for type {Type}", type);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{type}")]
        public async Task<ActionResult<NotificationPreferenceDto>> UpdatePreference(
            NotificationType type,
            UpdateNotificationPreferenceDto dto)
        {
            try
            {
                var userId = GetUserId();
                var preference = await _preferenceService.UpdateUserPreferenceAsync(userId, type, dto);
                return Ok(preference);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating notification preference for type {Type}", type);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("bulk")]
        public async Task<ActionResult<IEnumerable<NotificationPreferenceDto>>> BulkUpdatePreferences(BulkUpdatePreferencesDto dto)
        {
            try
            {
                var userId = GetUserId();
                var preferences = await _preferenceService.BulkUpdateUserPreferencesAsync(userId, dto);
                return Ok(preferences);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error bulk updating notification preferences");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("initialize")]
        public async Task<IActionResult> InitializeDefaultPreferences()
        {
            try
            {
                var userId = GetUserId();
                await _preferenceService.CreateDefaultPreferencesForUserAsync(userId);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing default notification preferences");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}