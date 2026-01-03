namespace StockTradePro.StockData.API.Models.DTOs
{
    public class StockSearchDto
    {
        public string? Query { get; set; }
        public string? Sector { get; set; }
        public string? Exchange { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SortBy { get; set; } = "symbol"; // symbol, name, price, change
        public string? SortOrder { get; set; } = "asc"; // asc, desc
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}
