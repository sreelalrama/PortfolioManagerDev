namespace StockTradePro.BlazorUI.Models.Portfolio
{
    public class PortfolioDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public double InitialValue { get; set; }
        public double CurrentValue { get; set; }
        public double TotalGainLoss { get; set; }
        public double TotalGainLossPercent { get; set; }
        public bool IsActive { get; set; }
        public bool IsPublic { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? LastCalculatedAt { get; set; }
        public int TotalHoldings { get; set; }
        public int TotalTransactions { get; set; }
        public List<HoldingDto> Holdings { get; set; } = new();
    }
}
