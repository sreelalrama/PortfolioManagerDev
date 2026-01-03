namespace StockTradePro.BlazorUI.Constants
{
    public static class ServiceConstants
    {
        // API Gateway URL - Single entry point for all services
        public const string ApiGatewayUrl = "http://localhost:5100";

        // SignalR Hub URLs - These typically bypass the gateway for direct connections
        public const string StockPriceHubUrl = "http://localhost:5100/stockPriceHub";
        public const string NotificationHubUrl = "http://localhost:5100/notificationHub";

        // API Endpoints - These will be routed through the gateway
        public static class ApiEndpoints
        {
            // Auth endpoints
            public const string Login = "/api/Auth/login";
            public const string Register = "/api/Auth/register";
            public const string RefreshToken = "/api/Auth/refresh-token";
            public const string Logout = "/api/Auth/logout";
            public const string ForgotPassword = "/api/Auth/forgot-password";
            public const string ResetPassword = "/api/Auth/reset-password";

            // User endpoints
            public const string UserProfile = "/api/Users/profile";
            public const string GetUser = "/api/Users/{0}";
            public const string ChangePassword = "/api/Users/change-password";

            // Stock endpoints
            public const string MarketOverview = "/api/Stocks/market-overview";
            public const string TrendingStocks = "/api/Stocks/trending";
            public const string TopGainers = "/api/Stocks/gainers";
            public const string TopLosers = "/api/Stocks/losers";
            public const string MostActive = "/api/Stocks/active";
            public const string GetStock = "/api/Stocks/{0}";
            public const string SearchStocks = "/api/Stocks/search";
            public const string GetStocks = "/api/Stocks";
            public const string GetSectors = "/api/Stocks/sectors";
            public const string GetExchanges = "/api/Stocks/exchanges";

            // Price endpoints
            public const string GetPrices = "/api/Prices/{0}";
            public const string GetCurrentPrice = "/api/Prices/{0}/current";
            public const string GetPriceChart = "/api/Prices/{0}/chart";

            // Portfolio endpoints
            public const string GetPortfolios = "/api/Portfolios";
            public const string GetPortfolio = "/api/Portfolios/{0}";
            public const string CreatePortfolio = "/api/Portfolios";
            public const string UpdatePortfolio = "/api/Portfolios/{0}";
            public const string DeletePortfolio = "/api/Portfolios/{0}";
            public const string RecalculatePortfolio = "/api/Portfolios/{0}/recalculate";
            public const string GetPublicPortfolios = "/api/Portfolios/public";

            // Transaction endpoints
            public const string GetPortfolioTransactions = "/api/Transactions/portfolio/{0}";
            public const string GetTransaction = "/api/Transactions/{0}";
            public const string CreateTransaction = "/api/Transactions";
            public const string DeleteTransaction = "/api/Transactions/{0}";
            public const string GetSymbolTransactions = "/api/Transactions/portfolio/{0}/symbol/{1}";

            // Watchlist endpoints
            public const string GetWatchlists = "/api/Watchlists";
            public const string GetWatchlist = "/api/Watchlists/{0}";
            public const string CreateWatchlist = "/api/Watchlists";
            public const string UpdateWatchlist = "/api/Watchlists/{0}";
            public const string DeleteWatchlist = "/api/Watchlists/{0}";
            public const string GetWatchlistItems = "/api/Watchlists/{0}/items";
            public const string AddWatchlistItem = "/api/Watchlists/{0}/items";
            public const string DeleteWatchlistItem = "/api/Watchlists/{0}/items/{1}";
            public const string UpdateItemSortOrder = "/api/Watchlists/{0}/items/{1}/sort-order";

            // Price Alert endpoints
            public const string GetPriceAlerts = "/api/watchlists/{0}/PriceAlerts";
            public const string GetPriceAlert = "/api/watchlists/{0}/PriceAlerts/{1}";
            public const string CreatePriceAlert = "/api/watchlists/{0}/PriceAlerts";
            public const string DeletePriceAlert = "/api/watchlists/{0}/PriceAlerts/{1}";
            public const string DisablePriceAlert = "/api/watchlists/{0}/PriceAlerts/{1}/disable";

            // Notification endpoints
            public const string GetNotifications = "/api/Notifications";
            public const string GetNotification = "/api/Notifications/{0}";
            public const string CreateNotification = "/api/Notifications";
            public const string DeleteNotification = "/api/Notifications/{0}";
            public const string MarkNotificationRead = "/api/Notifications/{0}/read";
            public const string MarkAllNotificationsRead = "/api/Notifications/mark-all-read";
            public const string GetUnreadCount = "/api/Notifications/unread-count";
            public const string CreatePriceAlertNotification = "/api/Notifications/price-alert";
            public const string CreatePortfolioUpdateNotification = "/api/Notifications/portfolio-update";

            // Notification Preferences endpoints
            public const string GetNotificationPreferences = "/api/NotificationPreferences";
            public const string GetNotificationPreference = "/api/NotificationPreferences/{0}";
            public const string UpdateNotificationPreference = "/api/NotificationPreferences/{0}";
            public const string BulkUpdatePreferences = "/api/NotificationPreferences/bulk";
            public const string InitializePreferences = "/api/NotificationPreferences/initialize";

            // Admin endpoints
            public const string SystemAnnouncement = "/api/admin/AdminNotifications/system-announcement";
            public const string BroadcastNotification = "/api/admin/AdminNotifications/broadcast";
            public const string TestNotification = "/api/admin/AdminNotifications/test-notification";
        }

        // Default Values
        public static class Defaults
        {
            public const int DefaultPageSize = 20;
            public const int DashboardItemCount = 10;
            public const int RecentTransactionsCount = 10;
            public const int TopStocksCount = 5;
            public const string DefaultPortfolioType = "Investment";
            public const string DefaultWatchlistName = "My Watchlist";
        }

        // Authentication
        public static class Auth
        {
            public const string AdminEmail = "admin@stocktradepro.com";
            public const string AdminPassword = "Admin123!";
            public const string BearerScheme = "Bearer";
            public const string AuthorizationHeader = "Authorization";
            public const string JwtClaimType = "sub";
            public const string UserIdClaimType = "userId";
        }

        // SignalR
        public static class SignalR
        {
            public static class StockPriceHub
            {
                public const string PriceUpdate = "PriceUpdate";
                public const string BulkPriceUpdate = "BulkPriceUpdate";
                public const string MarketStatus = "MarketStatus";
                public const string SubscriptionConfirmed = "SubscriptionConfirmed";
                public const string UnsubscriptionConfirmed = "UnsubscriptionConfirmed";
                public const string AllStocksSubscriptionConfirmed = "AllStocksSubscriptionConfirmed";
                public const string AllStocksUnsubscriptionConfirmed = "AllStocksUnsubscriptionConfirmed";
                public const string Error = "Error";
            }

            public static class NotificationHub
            {
                public const string ReceiveMessage = "ReceiveMessage";
                public const string ReceiveNotification = "ReceiveNotification";
                public const string UserGroupPrefix = "User_";
            }
        }

        // UI Constants
        public static class UI
        {
            public static class Colors
            {
                public const string GainColor = "text-success";
                public const string LossColor = "text-danger";
                public const string NeutralColor = "text-muted";
                public const string BuyBadgeColor = "bg-success";
                public const string SellBadgeColor = "bg-danger";
            }

            public static class Icons
            {
                public const string Home = "oi-home";
                public const string Stocks = "oi-graph";
                public const string Portfolio = "oi-briefcase";
                public const string Watchlist = "oi-eye";
                public const string Notifications = "oi-bell";
                public const string Settings = "oi-cog";
                public const string User = "oi-person";
                public const string Plus = "oi-plus";
                public const string Minus = "oi-minus";
                public const string Delete = "oi-trash";
                public const string Edit = "oi-pencil";
            }

            public static class Messages
            {
                public const string NoData = "No data available";
                public const string Loading = "Loading...";
                public const string Error = "An error occurred while loading data";
                public const string ServiceUnavailable = "Service is currently unavailable";
                public const string Unauthorized = "You are not authorized to perform this action";
                public const string NotFound = "The requested resource was not found";
            }
        }

        // Validation Constants
        public static class Validation
        {
            public const int MinPasswordLength = 6;
            public const int MaxNameLength = 100;
            public const int MaxDescriptionLength = 500;
            public const int MaxNotesLength = 500;
            public const int MaxSymbolLength = 10;
            public const double MinPrice = 0.01;
            public const double MinFees = 0.0;
            public const int MinQuantity = 1;
            public const int MaxQuantity = int.MaxValue;
        }

        // Date Formats
        public static class DateFormats
        {
            public const string ShortDate = "MM/dd/yyyy";
            public const string LongDate = "MMMM dd, yyyy";
            public const string DateTime = "MM/dd/yyyy HH:mm";
            public const string TimeOnly = "HH:mm";
            public const string ApiDateTime = "yyyy-MM-ddTHH:mm:ss";
        }

        // Number Formats
        public static class NumberFormats
        {
            public const string Currency = "C2";
            public const string Percentage = "P2";
            public const string Decimal2 = "F2";
            public const string Integer = "N0";
            public const string Volume = "N0";
        }
    }
}
