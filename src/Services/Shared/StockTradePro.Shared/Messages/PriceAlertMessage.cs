using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockTradePro.Shared.Messages
{
    public class PriceAlertMessage : BaseMessage
    {
        public int AlertId { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public string AlertType { get; set; } = string.Empty;
        public decimal TargetValue { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal ChangePercent { get; set; }
        public DateTime TriggeredAt { get; set; }
        public string? Notes { get; set; }
    }
}
