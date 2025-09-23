namespace StockTradePro.BlazorUI.Models.Portfolio
{
    public class CreatePortfolioDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public double InitialValue { get; set; }
        public bool IsPublic { get; set; }
    }
}
