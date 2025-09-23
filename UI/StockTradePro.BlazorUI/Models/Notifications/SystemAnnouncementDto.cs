namespace StockTradePro.BlazorUI.Models.Notifications
{
    public class SystemAnnouncementDto
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public List<string> UserIds { get; set; } = new();
    }

    public enum NotificationType
    {
        PriceAlert = 0,
        PortfolioUpdate = 1,
        SystemAnnouncement = 2,
        MarketNews = 3,
        WatchlistUpdate = 4
    }

    public enum NotificationStatus
    {
        Pending = 0,
        Sent = 1,
        Read = 2,
        Failed = 3
    }

    public enum DeliveryMethod
    {
        InApp = 0,
        Email = 1,
        Push = 2
    }
}
