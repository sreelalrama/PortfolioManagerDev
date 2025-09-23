using Microsoft.AspNetCore.Mvc;
using StockTradePro.NotificationService.API.DTOs;
using StockTradePro.NotificationService.API.Services;

namespace StockTradePro.NotificationService.API.Controllers
{
    [ApiController]
    [Route("api/admin/[controller]")]
    public class AdminNotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<AdminNotificationsController> _logger;

        public AdminNotificationsController(INotificationService notificationService, ILogger<AdminNotificationsController> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        [HttpPost("system-announcement")]
        public async Task<IActionResult> SendSystemAnnouncement([FromBody] SystemAnnouncementDto dto)
        {
            try
            {
                await _notificationService.SendSystemAnnouncementAsync(dto.Title, dto.Message, dto.UserIds);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending system announcement");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("broadcast")]
        public async Task<IActionResult> BroadcastNotification([FromBody] CreateNotificationDto dto)
        {
            try
            {
                var notification = await _notificationService.CreateNotificationAsync(dto);
                await _notificationService.SendRealTimeNotificationToAllAsync(notification);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error broadcasting notification");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("test-notification")]
        public async Task<IActionResult> SendTestNotification([FromBody] CreateNotificationDto dto)
        {
            try
            {
                var notification = await _notificationService.CreateNotificationAsync(dto);
                return Ok(new { message = "Test notification sent successfully", notificationId = notification.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending test notification");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}