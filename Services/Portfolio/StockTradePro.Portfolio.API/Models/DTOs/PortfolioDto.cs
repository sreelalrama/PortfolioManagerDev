namespace StockTradePro.Portfolio.API.Models.DTOs
{
    public class PortfolioDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Type { get; set; } = string.Empty;
        public decimal InitialValue { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal TotalGainLoss { get; set; }
        public decimal TotalGainLossPercent { get; set; }
        public bool IsActive { get; set; }
        public bool IsPublic { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? LastCalculatedAt { get; set; }

        // Summary data
        public int TotalHoldings { get; set; }
        public int TotalTransactions { get; set; }
        public List<HoldingDto> Holdings { get; set; } = new List<HoldingDto>();
    }
}
