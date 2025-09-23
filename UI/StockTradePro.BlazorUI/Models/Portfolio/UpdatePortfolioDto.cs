namespace StockTradePro.BlazorUI.Models.Portfolio
{
    public class UpdatePortfolioDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
    }
}
