namespace StockTradePro.BlazorUI.Models.Stocks
{
    public class StockSearchDto
    {
        public string Query { get; set; } = string.Empty;
        public string Sector { get; set; } = string.Empty;
        public string Exchange { get; set; } = string.Empty;
        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }
        public string SortBy { get; set; } = string.Empty;
        public string SortOrder { get; set; } = "asc";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
