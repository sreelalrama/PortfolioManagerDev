namespace StockTradePro.Shared.Common
{
    public enum NotificationType
    {
        PriceAlert,
        PortfolioUpdate,
        WatchlistUpdate,
        SystemAnnouncement,
        MarketNews
    }

    public enum AlertType
    {
        Above,
        Below,
        PercentageGain,
        PercentageLoss
    }

    public enum MessagePriority
    {
        Low,
        Normal,
        High,
        Critical
    }
}