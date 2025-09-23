
using Microsoft.EntityFrameworkCore;
using StockTradePro.NotificationService.API.Data;
using StockTradePro.NotificationService.API.DTOs;
using StockTradePro.NotificationService.API.Models;

namespace StockTradePro.NotificationService.API.Services
{
    public class UserNotificationPreferenceService : IUserNotificationPreferenceService
    {
        private readonly NotificationDbContext _context;
        private readonly ILogger<UserNotificationPreferenceService> _logger;

        public UserNotificationPreferenceService(NotificationDbContext context, ILogger<UserNotificationPreferenceService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<NotificationPreferenceDto>> GetUserPreferencesAsync(string userId)
        {
            var preferences = await _context.UserNotificationPreferences
                .Where(p => p.UserId == userId)
                .ToListAsync();

            // If user has no preferences, create defaults
            if (!preferences.Any())
            {
                await CreateDefaultPreferencesForUserAsync(userId);
                preferences = await _context.UserNotificationPreferences
                    .Where(p => p.UserId == userId)
                    .ToListAsync();
            }

            return preferences.Select(MapToDto);
        }

        public async Task<NotificationPreferenceDto?> GetUserPreferenceAsync(string userId, NotificationType type)
        {
            var preference = await _context.UserNotificationPreferences
                .Where(p => p.UserId == userId && p.Type == type)
                .FirstOrDefaultAsync();

            if (preference == null)
            {
                await CreateDefaultPreferencesForUserAsync(userId);
                preference = await _context.UserNotificationPreferences
                    .Where(p => p.UserId == userId && p.Type == type)
                    .FirstOrDefaultAsync();
            }

            return preference != null ? MapToDto(preference) : null;
        }

        public async Task<NotificationPreferenceDto> UpdateUserPreferenceAsync(string userId, NotificationType type, UpdateNotificationPreferenceDto dto)
        {
            var preference = await _context.UserNotificationPreferences
                .Where(p => p.UserId == userId && p.Type == type)
                .FirstOrDefaultAsync();

            if (preference == null)
            {
                await CreateDefaultPreferencesForUserAsync(userId);
                preference = await _context.UserNotificationPreferences
                    .Where(p => p.UserId == userId && p.Type == type)
                    .FirstOrDefaultAsync();
            }

            if (preference != null)
            {
                preference.InAppEnabled = dto.InAppEnabled;
                preference.EmailEnabled = dto.EmailEnabled;
                preference.PushEnabled = dto.PushEnabled;
                preference.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }

            return MapToDto(preference!);
        }

        public async Task<IEnumerable<NotificationPreferenceDto>> BulkUpdateUserPreferencesAsync(string userId, BulkUpdatePreferencesDto dto)
        {
            var existingPreferences = await _context.UserNotificationPreferences
                .Where(p => p.UserId == userId)
                .ToListAsync();

            if (!existingPreferences.Any())
            {
                await CreateDefaultPreferencesForUserAsync(userId);
                existingPreferences = await _context.UserNotificationPreferences
                    .Where(p => p.UserId == userId)
                    .ToListAsync();
            }

            foreach (var kvp in dto.Preferences)
            {
                var preference = existingPreferences.FirstOrDefault(p => p.Type == kvp.Key);
                if (preference != null)
                {
                    preference.InAppEnabled = kvp.Value.InAppEnabled;
                    preference.EmailEnabled = kvp.Value.EmailEnabled;
                    preference.PushEnabled = kvp.Value.PushEnabled;
                    preference.UpdatedAt = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();
            return existingPreferences.Select(MapToDto);
        }

        public async Task CreateDefaultPreferencesForUserAsync(string userId)
        {
            var existingPreferences = await _context.UserNotificationPreferences
                .Where(p => p.UserId == userId)
                .Select(p => p.Type)
                .ToListAsync();

            var notificationTypes = Enum.GetValues<NotificationType>();
            var newPreferences = new List<UserNotificationPreference>();

            foreach (var type in notificationTypes)
            {
                if (!existingPreferences.Contains(type))
                {
                    newPreferences.Add(new UserNotificationPreference
                    {
                        UserId = userId,
                        Type = type,
                        InAppEnabled = true,
                        EmailEnabled = type == NotificationType.PriceAlert || type == NotificationType.SystemAnnouncement,
                        PushEnabled = false,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }

            if (newPreferences.Any())
            {
                _context.UserNotificationPreferences.AddRange(newPreferences);
                await _context.SaveChangesAsync();
            }
        }

        private NotificationPreferenceDto MapToDto(UserNotificationPreference preference)
        {
            return new NotificationPreferenceDto
            {
                Id = preference.Id,
                UserId = preference.UserId,
                Type = preference.Type,
                InAppEnabled = preference.InAppEnabled,
                EmailEnabled = preference.EmailEnabled,
                PushEnabled = preference.PushEnabled,
                CreatedAt = preference.CreatedAt,
                UpdatedAt = preference.UpdatedAt
            };
        }
    }
}
