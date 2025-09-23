
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using StockTradePro.NotificationService.API.Data;
using StockTradePro.NotificationService.API.DTOs;
using StockTradePro.NotificationService.API.Models;
using StockTradePro.NotificationService.API.Hubs;

namespace StockTradePro.NotificationService.API.Services
{
    public class NotificationServiceImpl : INotificationService
    {
        private readonly NotificationDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IUserNotificationPreferenceService _preferenceService;
        private readonly IEmailService _emailService;
        private readonly ILogger<NotificationServiceImpl> _logger;

        public NotificationServiceImpl(
            NotificationDbContext context,
            IHubContext<NotificationHub> hubContext,
            IUserNotificationPreferenceService preferenceService,
            IEmailService emailService,
            ILogger<NotificationServiceImpl> logger)
        {
            _context = context;
            _hubContext = hubContext;
            _preferenceService = preferenceService;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto dto)
        {
            var notification = new Notification
            {
                UserId = dto.UserId,
                Type = dto.Type,
                Title = dto.Title,
                Message = dto.Message,
                Data = dto.Data,
                Method = dto.Method,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            var notificationDto = MapToDto(notification);

            // Send real-time notification
            await SendRealTimeNotificationAsync(dto.UserId, notificationDto);

            // Process based on user preferences
            await ProcessNotificationDelivery(notification);

            return notificationDto;
        }

        public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(string userId, int page = 1, int pageSize = 20)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return notifications.Select(MapToDto);
        }

        public async Task<NotificationDto?> GetNotificationAsync(int id, string userId)
        {
            var notification = await _context.Notifications
                .Where(n => n.Id == id && n.UserId == userId)
                .FirstOrDefaultAsync();

            return notification != null ? MapToDto(notification) : null;
        }

        public async Task<bool> MarkAsReadAsync(int id, string userId)
        {
            var notification = await _context.Notifications
                .Where(n => n.Id == id && n.UserId == userId)
                .FirstOrDefaultAsync();

            if (notification == null) return false;

            notification.Status = NotificationStatus.Read;
            notification.ReadAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkAllAsReadAsync(string userId)
        {
            await _context.Notifications
                .Where(n => n.UserId == userId && n.Status != NotificationStatus.Read)
                .ExecuteUpdateAsync(n => n
                    .SetProperty(p => p.Status, NotificationStatus.Read)
                    .SetProperty(p => p.ReadAt, DateTime.UtcNow));

            return true;
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && n.Status != NotificationStatus.Read)
                .CountAsync();
        }

        public async Task<bool> DeleteNotificationAsync(int id, string userId)
        {
            var notification = await _context.Notifications
                .Where(n => n.Id == id && n.UserId == userId)
                .FirstOrDefaultAsync();

            if (notification == null) return false;

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task SendPriceAlertNotificationAsync(PriceAlertRequestDto request)
        {
            var alertData = request.AlertData;
            var title = $"Price Alert: {alertData.Symbol}";
            var message = $"{alertData.Symbol} has reached your target. " +
                         $"Current price: ${alertData.CurrentPrice:F2} " +
                         $"({alertData.ChangePercent:+0.00;-0.00}%)";

            var notificationDto = new CreateNotificationDto
            {
                UserId = request.UserId,
                Type = NotificationType.PriceAlert,
                Title = title,
                Message = message,
                Data = JsonSerializer.Serialize(alertData)
            };

            await CreateNotificationAsync(notificationDto);
        }

        public async Task SendPortfolioUpdateNotificationAsync(PortfolioUpdateRequestDto request)
        {
            var portfolioData = request.PortfolioData;
            var title = $"Portfolio Update: {portfolioData.PortfolioName}";
            var changeText = portfolioData.DayChange >= 0 ? "gained" : "lost";
            var message = $"Your portfolio has {changeText} ${Math.Abs(portfolioData.DayChange):F2} " +
                         $"({portfolioData.DayChangePercent:+0.00;-0.00}%) today. " +
                         $"Current value: ${portfolioData.TotalValue:F2}";

            var notificationDto = new CreateNotificationDto
            {
                UserId = request.UserId,
                Type = NotificationType.PortfolioUpdate,
                Title = title,
                Message = message,
                Data = JsonSerializer.Serialize(portfolioData)
            };

            await CreateNotificationAsync(notificationDto);
        }

        public async Task SendSystemAnnouncementAsync(string title, string message, IEnumerable<string>? userIds = null)
        {
            var notifications = new List<Notification>();

            if (userIds == null)
            {
                // Send to all users
                var allUserIds = await GetAllUserIdsAsync();
                userIds = allUserIds;
            }

            foreach (var userId in userIds)
            {
                notifications.Add(new Notification
                {
                    UserId = userId,
                    Type = NotificationType.SystemAnnouncement,
                    Title = title,
                    Message = message,
                    CreatedAt = DateTime.UtcNow
                });
            }

            _context.Notifications.AddRange(notifications);
            await _context.SaveChangesAsync();

            // Send real-time notifications
            foreach (var notification in notifications)
            {
                await SendRealTimeNotificationAsync(notification.UserId, MapToDto(notification));
            }
        }

        public async Task SendRealTimeNotificationAsync(string userId, NotificationDto notification)
        {
            try
            {
                await _hubContext.Clients.Group($"User_{userId}")
                    .SendAsync("ReceiveNotification", notification);

                _logger.LogInformation("Real-time notification sent to user {UserId}: {Title}",
                    userId, notification.Title);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send real-time notification to user {UserId}", userId);
            }
        }

        public async Task SendRealTimeNotificationToAllAsync(NotificationDto notification)
        {
            try
            {
                await _hubContext.Clients.All.SendAsync("ReceiveNotification", notification);
                _logger.LogInformation("Real-time notification sent to all users: {Title}", notification.Title);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send real-time notification to all users");
            }
        }

        private async Task ProcessNotificationDelivery(Notification notification)
        {
            try
            {
                var preferences = await _preferenceService.GetUserPreferencesAsync(notification.UserId);
                var typePreference = preferences.FirstOrDefault(p => p.Type == notification.Type);

                if (typePreference == null)
                {
                    _logger.LogWarning("No preferences found for user {UserId} and type {Type}",
                        notification.UserId, notification.Type);
                    return;
                }

                // Send email if enabled
                if (typePreference.EmailEnabled)
                {
                    await _emailService.SendNotificationEmailAsync(
                        notification.UserId,
                        notification.Title,
                        notification.Message);
                }

                // Future: Send push notification if enabled
                if (typePreference.PushEnabled)
                {
                    _logger.LogInformation("Push notification would be sent to user {UserId}",
                        notification.UserId);
                    // TODO: Implement push notification logic
                }

                notification.Status = NotificationStatus.Sent;
                notification.SentAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing notification delivery for notification {NotificationId}",
                    notification.Id);

                notification.Status = NotificationStatus.Failed;
                notification.ErrorMessage = ex.Message;
                notification.RetryCount = (notification.RetryCount ?? 0) + 1;
                await _context.SaveChangesAsync();
            }
        }

        private async Task<IEnumerable<string>> GetAllUserIdsAsync()
        {
            // Get unique user IDs from existing notifications
            return await _context.Notifications
                .Select(n => n.UserId)
                .Distinct()
                .ToListAsync();
        }

        private NotificationDto MapToDto(Notification notification)
        {
            return new NotificationDto
            {
                Id = notification.Id,
                UserId = notification.UserId,
                Type = notification.Type,
                Title = notification.Title,
                Message = notification.Message,
                Data = notification.Data,
                Status = notification.Status,
                Method = notification.Method,
                CreatedAt = notification.CreatedAt,
                SentAt = notification.SentAt,
                ReadAt = notification.ReadAt
            };
        }
    }
}