using StockTradePro.BlazorUI.Models.Notifications;
using StockTradePro.BlazorUI.Services;

namespace StockTradePro.BlazorUI.Services.Mocks
{
    public class MockNotificationService : INotificationService
    {
        private readonly List<NotificationDto> _notifications;
        private readonly List<NotificationPreferenceDto> _preferences;
        private int _nextId;
        private string _currentUserId = "1"; // Default to user "1" for testing

        public MockNotificationService()
        {
            _nextId = 1;
            _notifications = GenerateMockNotifications();
            _preferences = GenerateMockPreferences();
        }

        private List<NotificationDto> GenerateMockNotifications()
        {
            return new List<NotificationDto>
            {
                new NotificationDto
                {
                    Id = _nextId++,
                    UserId = "1",
                    Type = NotificationType.PriceAlert,
                    Title = "Price Alert: AAPL",
                    Message = "Apple (AAPL) has reached your target price of $150.00",
                    Data = "{\"symbol\":\"AAPL\",\"price\":150.00,\"alertId\":123}",
                    Status = NotificationStatus.Sent,
                    Method = DeliveryMethod.InApp,
                    CreatedAt = DateTime.UtcNow.AddHours(-2),
                    SentAt = DateTime.UtcNow.AddHours(-2)
                },
                new NotificationDto
                {
                    Id = _nextId++,
                    UserId = "1",
                    Type = NotificationType.PortfolioUpdate,
                    Title = "Portfolio Performance",
                    Message = "Your portfolio gained 2.5% today (+$1,250)",
                    Data = "{\"change\":2.5,\"amount\":1250,\"portfolioId\":456}",
                    Status = NotificationStatus.Read,
                    Method = DeliveryMethod.InApp,
                    CreatedAt = DateTime.UtcNow.AddHours(-4),
                    SentAt = DateTime.UtcNow.AddHours(-4),
                    ReadAt = DateTime.UtcNow.AddHours(-3)
                },
                new NotificationDto
                {
                    Id = _nextId++,
                    UserId = "1",
                    Type = NotificationType.SystemAnnouncement,
                    Title = "System Maintenance",
                    Message = "Scheduled maintenance tonight from 2-4 AM EST. Limited functionality expected.",
                    Data = "{\"maintenanceStart\":\"2025-09-24T02:00:00Z\",\"maintenanceEnd\":\"2025-09-24T04:00:00Z\"}",
                    Status = NotificationStatus.Sent,
                    Method = DeliveryMethod.InApp,
                    CreatedAt = DateTime.UtcNow.AddHours(-6),
                    SentAt = DateTime.UtcNow.AddHours(-6)
                },
                new NotificationDto
                {
                    Id = _nextId++,
                    UserId = "1",
                    Type = NotificationType.MarketNews,
                    Title = "Market News: Tech Sector",
                    Message = "Major tech stocks rally on positive earnings reports",
                    Data = "{\"sector\":\"technology\",\"newsId\":789}",
                    Status = NotificationStatus.Sent,
                    Method = DeliveryMethod.Email,
                    CreatedAt = DateTime.UtcNow.AddHours(-8),
                    SentAt = DateTime.UtcNow.AddHours(-8)
                },
                new NotificationDto
                {
                    Id = _nextId++,
                    UserId = "1",
                    Type = NotificationType.WatchlistUpdate,
                    Title = "Watchlist Alert: TSLA",
                    Message = "Tesla (TSLA) moved 5% in the last hour",
                    Data = "{\"symbol\":\"TSLA\",\"change\":5.2,\"watchlistId\":321}",
                    Status = NotificationStatus.Read,
                    Method = DeliveryMethod.InApp,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    SentAt = DateTime.UtcNow.AddDays(-1),
                    ReadAt = DateTime.UtcNow.AddHours(-12)
                },
                new NotificationDto
                {
                    Id = _nextId++,
                    UserId = "2",
                    Type = NotificationType.PriceAlert,
                    Title = "Price Alert: GOOGL",
                    Message = "Alphabet (GOOGL) has reached your target price of $140.00",
                    Data = "{\"symbol\":\"GOOGL\",\"price\":140.00,\"alertId\":124}",
                    Status = NotificationStatus.Sent,
                    Method = DeliveryMethod.InApp,
                    CreatedAt = DateTime.UtcNow.AddHours(-1),
                    SentAt = DateTime.UtcNow.AddHours(-1)
                }
            };
        }

        private List<NotificationPreferenceDto> GenerateMockPreferences()
        {
            var preferences = new List<NotificationPreferenceDto>();
            var notificationTypes = Enum.GetValues<NotificationType>();

            foreach (var type in notificationTypes)
            {
                preferences.Add(new NotificationPreferenceDto
                {
                    Id = preferences.Count + 1,
                    UserId = "1",
                    Type = type,
                    InAppEnabled = true,
                    EmailEnabled = type == NotificationType.SystemAnnouncement || type == NotificationType.MarketNews,
                    PushEnabled = type == NotificationType.PriceAlert || type == NotificationType.PortfolioUpdate,
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    UpdatedAt = DateTime.UtcNow.AddDays(-5)
                });

                // Add preferences for user "2" as well
                preferences.Add(new NotificationPreferenceDto
                {
                    Id = preferences.Count + 1,
                    UserId = "2",
                    Type = type,
                    InAppEnabled = true,
                    EmailEnabled = true,
                    PushEnabled = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-15),
                    UpdatedAt = DateTime.UtcNow.AddDays(-2)
                });
            }

            return preferences;
        }

        public async Task<List<NotificationDto>> GetNotificationsAsync(int page = 1, int pageSize = 20)
        {
            await Task.Delay(200); // Simulate API delay

            var userNotifications = _notifications
                .Where(n => n.UserId == _currentUserId)
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return userNotifications;
        }

        public async Task<NotificationDto?> GetNotificationAsync(int id)
        {
            await Task.Delay(100); // Simulate API delay

            return _notifications
                .FirstOrDefault(n => n.Id == id && n.UserId == _currentUserId);
        }

        public async Task<NotificationDto?> CreateNotificationAsync(CreateNotificationDto createDto)
        {
            await Task.Delay(300); // Simulate API delay

            var notification = new NotificationDto
            {
                Id = _nextId++,
                UserId = createDto.UserId,
                Type = createDto.Type,
                Title = createDto.Title,
                Message = createDto.Message,
                Data = createDto.Data,
                Status = NotificationStatus.Pending,
                Method = createDto.Method,
                CreatedAt = DateTime.UtcNow
            };

            // Simulate sending the notification
            await Task.Delay(100);
            notification.Status = NotificationStatus.Sent;
            notification.SentAt = DateTime.UtcNow;

            _notifications.Add(notification);
            return notification;
        }

        public async Task<bool> DeleteNotificationAsync(int id)
        {
            await Task.Delay(200); // Simulate API delay

            var notification = _notifications
                .FirstOrDefault(n => n.Id == id && n.UserId == _currentUserId);

            if (notification != null)
            {
                _notifications.Remove(notification);
                return true;
            }

            return false;
        }

        public async Task<bool> MarkNotificationReadAsync(int id)
        {
            await Task.Delay(150); // Simulate API delay

            var notification = _notifications
                .FirstOrDefault(n => n.Id == id && n.UserId == _currentUserId);

            if (notification != null && notification.Status == NotificationStatus.Sent)
            {
                notification.Status = NotificationStatus.Read;
                notification.ReadAt = DateTime.UtcNow;
                return true;
            }

            return false;
        }

        public async Task<bool> MarkAllNotificationsReadAsync()
        {
            await Task.Delay(300); // Simulate API delay

            var unreadNotifications = _notifications
                .Where(n => n.UserId == _currentUserId && n.Status == NotificationStatus.Sent)
                .ToList();

            foreach (var notification in unreadNotifications)
            {
                notification.Status = NotificationStatus.Read;
                notification.ReadAt = DateTime.UtcNow;
            }

            return true;
        }

        public async Task<int> GetUnreadCountAsync()
        {
            await Task.Delay(100); // Simulate API delay

            return _notifications
                .Count(n => n.UserId == _currentUserId && n.Status == NotificationStatus.Sent);
        }

        public async Task<List<NotificationPreferenceDto>> GetNotificationPreferencesAsync()
        {
            await Task.Delay(200); // Simulate API delay

            return _preferences
                .Where(p => p.UserId == _currentUserId)
                .OrderBy(p => p.Type)
                .ToList();
        }

        public async Task<NotificationPreferenceDto?> GetNotificationPreferenceAsync(NotificationType type)
        {
            await Task.Delay(100); // Simulate API delay

            return _preferences
                .FirstOrDefault(p => p.UserId == _currentUserId && p.Type == type);
        }

        public async Task<NotificationPreferenceDto?> UpdateNotificationPreferenceAsync(NotificationType type, UpdateNotificationPreferenceDto updateDto)
        {
            await Task.Delay(250); // Simulate API delay

            var preference = _preferences
                .FirstOrDefault(p => p.UserId == _currentUserId && p.Type == type);

            if (preference != null)
            {
                preference.InAppEnabled = updateDto.InAppEnabled;
                preference.EmailEnabled = updateDto.EmailEnabled;
                preference.PushEnabled = updateDto.PushEnabled;
                preference.UpdatedAt = DateTime.UtcNow;

                return preference;
            }

            return null;
        }

        public async Task<bool> InitializePreferencesAsync()
        {
            await Task.Delay(400); // Simulate API delay

            // Check if preferences already exist for current user
            var existingPreferences = _preferences
                .Where(p => p.UserId == _currentUserId)
                .ToList();

            if (existingPreferences.Any())
            {
                return true; // Already initialized
            }

            // Create default preferences for all notification types
            var notificationTypes = Enum.GetValues<NotificationType>();
            var newPreferences = new List<NotificationPreferenceDto>();

            foreach (var type in notificationTypes)
            {
                var preference = new NotificationPreferenceDto
                {
                    Id = _preferences.Count + newPreferences.Count + 1,
                    UserId = _currentUserId,
                    Type = type,
                    InAppEnabled = true, // Default: all in-app notifications enabled
                    EmailEnabled = type == NotificationType.SystemAnnouncement, // Default: only system announcements via email
                    PushEnabled = type == NotificationType.PriceAlert, // Default: only price alerts via push
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                newPreferences.Add(preference);
            }

            _preferences.AddRange(newPreferences);
            return true;
        }

        // Helper method to set current user (useful for testing different users)
        public void SetCurrentUser(string userId)
        {
            _currentUserId = userId;
        }

        // Helper method to add system announcement to all users
        public async Task<List<NotificationDto>> CreateSystemAnnouncementAsync(SystemAnnouncementDto announcementDto)
        {
            await Task.Delay(500); // Simulate API delay

            var notifications = new List<NotificationDto>();

            foreach (var userId in announcementDto.UserIds)
            {
                var notification = new NotificationDto
                {
                    Id = _nextId++,
                    UserId = userId,
                    Type = NotificationType.SystemAnnouncement,
                    Title = announcementDto.Title,
                    Message = announcementDto.Message,
                    Data = "{}",
                    Status = NotificationStatus.Sent,
                    Method = DeliveryMethod.InApp,
                    CreatedAt = DateTime.UtcNow,
                    SentAt = DateTime.UtcNow
                };

                _notifications.Add(notification);
                notifications.Add(notification);
            }

            return notifications;
        }
    }
}