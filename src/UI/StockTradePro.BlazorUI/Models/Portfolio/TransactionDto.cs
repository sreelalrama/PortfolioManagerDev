namespace StockTradePro.BlazorUI.Models.Portfolio
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public int PortfolioId { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // BUY or SELL
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double TotalAmount { get; set; }
        public double Fees { get; set; }
        public double NetAmount { get; set; }
        public string Notes { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
