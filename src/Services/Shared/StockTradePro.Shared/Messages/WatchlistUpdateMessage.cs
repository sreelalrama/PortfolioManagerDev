using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockTradePro.Shared.Messages
{
    public class WatchlistUpdateMessage : BaseMessage
    {
        public string WatchlistName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty; // "Added", "Removed", "Created", "Deleted"
        public string? Symbol { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
