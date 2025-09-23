using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace StockTradePro.NotificationService.API.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly ILogger<NotificationHub> _logger;

        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
                _logger.LogInformation("User {UserId} connected to notification hub with connection {ConnectionId}", 
                    userId, Context.ConnectionId);
            }
            else
            {
                _logger.LogWarning("User connected without valid user ID. Connection: {ConnectionId}", 
                    Context.ConnectionId);
            }
            
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = GetUserId();
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
                _logger.LogInformation("User {UserId} disconnected from notification hub. Connection: {ConnectionId}", 
                    userId, Context.ConnectionId);
            }

            if (exception != null)
            {
                _logger.LogError(exception, "User disconnected with error. Connection: {ConnectionId}", 
                    Context.ConnectionId);
            }
            
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinUserGroup(string userId)
        {
            var currentUserId = GetUserId();
            if (currentUserId == userId || string.IsNullOrEmpty(currentUserId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
                _logger.LogInformation("User {UserId} joined group User_{GroupUserId}", 
                    currentUserId ?? "Anonymous", userId);
            }
            else
            {
                _logger.LogWarning("User {UserId} attempted to join group for different user {GroupUserId}", 
                    currentUserId, userId);
            }
        }

        public async Task LeaveUserGroup(string userId)
        {
            var currentUserId = GetUserId();
            if (currentUserId == userId || string.IsNullOrEmpty(currentUserId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
                _logger.LogInformation("User {UserId} left group User_{GroupUserId}", 
                    currentUserId ?? "Anonymous", userId);
            }
        }

        public async Task SendToUser(string userId, string message)
        {
            await Clients.Group($"User_{userId}").SendAsync("ReceiveMessage", message);
        }

        public async Task SendToAll(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        private string? GetUserId()
        {
            // Try different claim types for user ID
            return Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
                   Context.User?.FindFirst("sub")?.Value ??
                   Context.User?.FindFirst("userId")?.Value;
        }
    }
}