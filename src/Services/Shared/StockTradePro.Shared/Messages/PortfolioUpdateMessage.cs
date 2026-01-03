using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockTradePro.Shared.Messages
{
    public class PortfolioUpdateMessage : BaseMessage
    {
        public string PortfolioName { get; set; } = string.Empty;
        public decimal TotalValue { get; set; }
        public decimal DayChange { get; set; }
        public decimal DayChangePercent { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
