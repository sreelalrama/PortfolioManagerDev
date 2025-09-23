namespace StockTradePro.BlazorUI.Models.Notifications
{
    public class UpdateNotificationPreferenceDto
    {
        public bool InAppEnabled { get; set; }
        public bool EmailEnabled { get; set; }
        public bool PushEnabled { get; set; }
    }
}
