namespace StockTradePro.Portfolio.API.Models.DTOs
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public int PortfolioId { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Fees { get; set; }
        public decimal NetAmount { get; set; }
        public string? Notes { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}