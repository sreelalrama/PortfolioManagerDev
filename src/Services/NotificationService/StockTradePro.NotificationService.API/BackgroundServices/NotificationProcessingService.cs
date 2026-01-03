// BackgroundServices/NotificationProcessingService.cs (Optional future enhancement)
using StockTradePro.NotificationService.API.Data;
using StockTradePro.NotificationService.API.Models;
using StockTradePro.NotificationService.API.Services;
using Microsoft.EntityFrameworkCore;

namespace StockTradePro.NotificationService.API.BackgroundServices
{
    public class NotificationProcessingService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NotificationProcessingService> _logger;
        private readonly TimeSpan _processingInterval = TimeSpan.FromMinutes(5); // Process every 5 minutes

        public NotificationProcessingService(IServiceProvider serviceProvider, ILogger<NotificationProcessingService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Notification Processing Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessFailedNotifications();
                    await CleanupOldNotifications();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred in notification processing service");
                }

                await Task.Delay(_processingInterval, stoppingToken);
            }

            _logger.LogInformation("Notification Processing Service stopped");
        }

        private async Task ProcessFailedNotifications()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            // Get failed notifications that haven't exceeded retry limit
            var failedNotifications = await context.Notifications
                .Where(n => n.Status == NotificationStatus.Failed && n.RetryCount < 3)
                .Where(n => n.CreatedAt > DateTime.UtcNow.AddDays(-1)) // Only retry within 24 hours
                .ToListAsync();

            foreach (var notification in failedNotifications)
            {
                try
                {
                    _logger.LogInformation("Retrying failed notification {NotificationId}", notification.Id);

                    // Retry email delivery if it was supposed to be an email notification
                    if (notification.Method == DeliveryMethod.Email)
                    {
                        await emailService.SendNotificationEmailAsync(
                            notification.UserId,
                            notification.Title,
                            notification.Message);
                    }

                    // Mark as sent
                    notification.Status = NotificationStatus.Sent;
                    notification.SentAt = DateTime.UtcNow;
                    notification.ErrorMessage = null;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Retry failed for notification {NotificationId}", notification.Id);

                    notification.RetryCount = (notification.RetryCount ?? 0) + 1;
                    notification.ErrorMessage = ex.Message;

                    // If max retries exceeded, log and give up
                    if (notification.RetryCount >= 3)
                    {
                        _logger.LogWarning("Max retries exceeded for notification {NotificationId}", notification.Id);
                    }
                }
            }

            await context.SaveChangesAsync();
        }

        private async Task CleanupOldNotifications()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();

            // Delete read notifications older than 30 days
            var cutoffDate = DateTime.UtcNow.AddDays(-30);

            var oldNotifications = await context.Notifications
                .Where(n => n.Status == NotificationStatus.Read && n.ReadAt < cutoffDate)
                .ToListAsync();

            if (oldNotifications.Any())
            {
                context.Notifications.RemoveRange(oldNotifications);
                await context.SaveChangesAsync();

                _logger.LogInformation("Cleaned up {Count} old notifications", oldNotifications.Count);
            }

            // Delete very old unread notifications (older than 90 days)
            var veryOldCutoff = DateTime.UtcNow.AddDays(-90);

            var veryOldNotifications = await context.Notifications
                .Where(n => n.CreatedAt < veryOldCutoff)
                .ToListAsync();

            if (veryOldNotifications.Any())
            {
                context.Notifications.RemoveRange(veryOldNotifications);
                await context.SaveChangesAsync();

                _logger.LogInformation("Cleaned up {Count} very old notifications", veryOldNotifications.Count);
            }
        }
    }
}