using Microsoft.AspNetCore.Mvc;
using StockTradePro.StockData.API.Models.DTOs;
using StockTradePro.StockData.API.Services;

namespace StockTradePro.StockData.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PricesController : ControllerBase
    {
        private readonly IStockService _stockService;
        private readonly ILogger<PricesController> _logger;

        public PricesController(IStockService stockService, ILogger<PricesController> logger)
        {
            _stockService = stockService;
            _logger = logger;
        }

        /// <summary>
        /// Get price history for a stock
        /// </summary>
        [HttpGet("{symbol}")]
        public async Task<ActionResult<List<StockPriceDto>>> GetStockPrices(
            string symbol,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                if (string.IsNullOrEmpty(symbol))
                {
                    return BadRequest("Stock symbol is required");
                }

                // Default to last 30 days if no date range specified
                if (!fromDate.HasValue && !toDate.HasValue)
                {
                    fromDate = DateTime.UtcNow.AddDays(-30);
                    toDate = DateTime.UtcNow;
                }

                var prices = await _stockService.GetStockPricesAsync(symbol, fromDate, toDate);

                if (!prices.Any())
                {
                    return NotFound($"No price data found for stock '{symbol}'");
                }

                return Ok(prices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving prices for stock: {Symbol}", symbol);
                return StatusCode(500, "An error occurred while retrieving stock prices");
            }
        }

        /// <summary>
        /// Get current price for a stock
        /// </summary>
        [HttpGet("{symbol}/current")]
        public async Task<ActionResult<StockPriceDto>> GetCurrentPrice(string symbol)
        {
            try
            {
                if (string.IsNullOrEmpty(symbol))
                {
                    return BadRequest("Stock symbol is required");
                }

                var stock = await _stockService.GetStockBySymbolAsync(symbol);

                if (stock == null)
                {
                    return NotFound($"Stock with symbol '{symbol}' not found");
                }

                var currentPrice = new StockPriceDto
                {
                    Symbol = stock.Symbol,
                    CurrentPrice = stock.CurrentPrice,
                    PriceChange = stock.PriceChange,
                    PriceChangePercent = stock.PriceChangePercent,
                    Volume = stock.Volume,
                    LastUpdated = stock.LastUpdated,
                    Date = DateTime.UtcNow.Date
                };

                return Ok(currentPrice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current price for stock: {Symbol}", symbol);
                return StatusCode(500, "An error occurred while retrieving current price");
            }
        }

        /// <summary>
        /// Get price history chart data
        /// </summary>
        [HttpGet("{symbol}/chart")]
        public async Task<ActionResult<object>> GetPriceChart(
            string symbol,
            [FromQuery] string period = "1M")
        {
            try
            {
                if (string.IsNullOrEmpty(symbol))
                {
                    return BadRequest("Stock symbol is required");
                }

                DateTime fromDate;
                var toDate = DateTime.UtcNow;

                // Parse period parameter
                fromDate = period.ToUpper() switch
                {
                    "1D" => toDate.AddDays(-1),
                    "5D" => toDate.AddDays(-5),
                    "1M" => toDate.AddMonths(-1),
                    "3M" => toDate.AddMonths(-3),
                    "6M" => toDate.AddMonths(-6),
                    "1Y" => toDate.AddYears(-1),
                    "2Y" => toDate.AddYears(-2),
                    "5Y" => toDate.AddYears(-5),
                    _ => toDate.AddMonths(-1)
                };

                var prices = await _stockService.GetStockPricesAsync(symbol, fromDate, toDate);

                if (!prices.Any())
                {
                    return NotFound($"No price data found for stock '{symbol}' in the specified period");
                }

                // Format data for charting
                var chartData = prices.Select(p => new
                {
                    date = p.Date.ToString("yyyy-MM-dd"),
                    timestamp = ((DateTimeOffset)p.Date).ToUnixTimeSeconds(),
                    open = p.OpenPrice,
                    high = p.HighPrice,
                    low = p.LowPrice,
                    close = p.ClosePrice,
                    volume = p.Volume
                }).ToList();

                return Ok(new
                {
                    symbol = symbol.ToUpper(),
                    period = period,
                    data = chartData,
                    count = chartData.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving chart data for stock: {Symbol}", symbol);
                return StatusCode(500, "An error occurred while retrieving chart data");
            }
        }
    }
}