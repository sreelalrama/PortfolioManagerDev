namespace StockTradePro.BlazorUI.Models.Portfolio
{
    public class CreateTransactionDto
    {
        public int PortfolioId { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // BUY or SELL
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double Fees { get; set; }
        public string Notes { get; set; } = string.Empty;
        public DateTime? TransactionDate { get; set; }
    }
}
