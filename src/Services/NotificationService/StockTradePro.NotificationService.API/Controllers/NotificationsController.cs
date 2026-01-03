using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using StockTradePro.NotificationService.API.DTOs;
using StockTradePro.NotificationService.API.Services;

namespace StockTradePro.NotificationService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(INotificationService notificationService, ILogger<NotificationsController> logger)
        {
            _notificationService = notificationService;
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
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetNotifications(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var userId = GetUserId();
                var notifications = await _notificationService.GetUserNotificationsAsync(userId, page, pageSize);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notifications");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NotificationDto>> GetNotification(int id)
        {
            try
            {
                var userId = GetUserId();
                var notification = await _notificationService.GetNotificationAsync(id, userId);

                if (notification == null)
                    return NotFound($"Notification with ID {id} not found");

                return Ok(notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notification {NotificationId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<NotificationDto>> CreateNotification(CreateNotificationDto dto)
        {
            try
            {
                var notification = await _notificationService.CreateNotificationAsync(dto);
                return CreatedAtAction(nameof(GetNotification), new { id = notification.Id }, notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notification");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPatch("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            try
            {
                var userId = GetUserId();
                var success = await _notificationService.MarkAsReadAsync(id, userId);

                if (!success)
                    return NotFound($"Notification with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification {NotificationId} as read", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPatch("mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            try
            {
                var userId = GetUserId();
                await _notificationService.MarkAllAsReadAsync(userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking all notifications as read");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            try
            {
                var userId = GetUserId();
                var count = await _notificationService.GetUnreadCountAsync(userId);
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread count");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            try
            {
                var userId = GetUserId();
                var success = await _notificationService.DeleteNotificationAsync(id, userId);

                if (!success)
                    return NotFound($"Notification with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification {NotificationId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("price-alert")]
        public async Task<IActionResult> SendPriceAlertNotification(PriceAlertRequestDto dto)
        {
            try
            {
                await _notificationService.SendPriceAlertNotificationAsync(dto);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending price alert notification");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("portfolio-update")]
        public async Task<IActionResult> SendPortfolioUpdateNotification(PortfolioUpdateRequestDto dto)
        {
            try
            {
                await _notificationService.SendPortfolioUpdateNotificationAsync(dto);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending portfolio update notification");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}