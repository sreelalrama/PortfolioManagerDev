using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockTradePro.Shared.Messages
{
    public abstract class BaseMessage
    {
        public string MessageId { get; set; } = Guid.NewGuid().ToString();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? CorrelationId { get; set; }
        public string? UserId { get; set; }
    }
}
