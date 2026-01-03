
namespace StockTradePro.NotificationService.API.Services
{
    public interface IEmailService
    {
        Task SendNotificationEmailAsync(string userId, string subject, string message);
        Task SendBulkNotificationEmailAsync(IEnumerable<string> userIds, string subject, string message);
        Task SendEmailAsync(string toEmail, string subject, string message);
    }
}