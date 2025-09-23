using Microsoft.AspNetCore.Mvc;
using StockTradePro.StockData.API.Models.DTOs;
using StockTradePro.StockData.API.Services;

namespace StockTradePro.StockData.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StocksController : ControllerBase
    {
        private readonly IStockService _stockService;
        private readonly ILogger<StocksController> _logger;

        public StocksController(IStockService stockService, ILogger<StocksController> logger)
        {
            _stockService = stockService;
            _logger = logger;
        }

        /// <summary>
        /// Get all stocks with optional filtering and pagination
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<StockDto>>> GetStocks([FromQuery] StockSearchDto searchDto)
        {
            try
            {
                var result = await _stockService.GetStocksAsync(searchDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving stocks");
                return StatusCode(500, "An error occurred while retrieving stocks");
            }
        }

        /// <summary>
        /// Get stock by symbol
        /// </summary>
        [HttpGet("{symbol}")]
        public async Task<ActionResult<StockDto>> GetStock(string symbol)
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

                return Ok(stock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving stock with symbol: {Symbol}", symbol);
                return StatusCode(500, "An error occurred while retrieving the stock");
            }
        }

        /// <summary>
        /// Search stocks by query
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<PaginatedResult<StockDto>>> SearchStocks([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var searchDto = new StockSearchDto
                {
                    Query = query,
                    Page = page,
                    PageSize = Math.Min(pageSize, 100) // Limit page size
                };

                var result = await _stockService.GetStocksAsync(searchDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching stocks with query: {Query}", query);
                return StatusCode(500, "An error occurred while searching stocks");
            }
        }

        /// <summary>
        /// Get trending stocks (most active by volume)
        /// </summary>
        [HttpGet("trending")]
        public async Task<ActionResult<List<StockDto>>> GetTrendingStocks([FromQuery] int count = 10)
        {
            try
            {
                if (count <= 0 || count > 50)
                {
                    return BadRequest("Count must be between 1 and 50");
                }

                var stocks = await _stockService.GetTrendingStocksAsync(count);
                return Ok(stocks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving trending stocks");
                return StatusCode(500, "An error occurred while retrieving trending stocks");
            }
        }

        /// <summary>
        /// Get top gainers
        /// </summary>
        [HttpGet("gainers")]
        public async Task<ActionResult<List<StockDto>>> GetTopGainers([FromQuery] int count = 10)
        {
            try
            {
                if (count <= 0 || count > 50)
                {
                    return BadRequest("Count must be between 1 and 50");
                }

                var stocks = await _stockService.GetTopGainersAsync(count);
                return Ok(stocks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving top gainers");
                return StatusCode(500, "An error occurred while retrieving top gainers");
            }
        }

        /// <summary>
        /// Get top losers
        /// </summary>
        [HttpGet("losers")]
        public async Task<ActionResult<List<StockDto>>> GetTopLosers([FromQuery] int count = 10)
        {
            try
            {
                if (count <= 0 || count > 50)
                {
                    return BadRequest("Count must be between 1 and 50");
                }

                var stocks = await _stockService.GetTopLosersAsync(count);
                return Ok(stocks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving top losers");
                return StatusCode(500, "An error occurred while retrieving top losers");
            }
        }

        /// <summary>
        /// Get most active stocks
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<List<StockDto>>> GetMostActive([FromQuery] int count = 10)
        {
            try
            {
                if (count <= 0 || count > 50)
                {
                    return BadRequest("Count must be between 1 and 50");
                }

                var stocks = await _stockService.GetMostActiveAsync(count);
                return Ok(stocks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving most active stocks");
                return StatusCode(500, "An error occurred while retrieving most active stocks");
            }
        }

        /// <summary>
        /// Get market overview
        /// </summary>
        [HttpGet("market-overview")]
        public async Task<ActionResult<MarketOverviewDto>> GetMarketOverview()
        {
            try
            {
                var overview = await _stockService.GetMarketOverviewAsync();
                return Ok(overview);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving market overview");
                return StatusCode(500, "An error occurred while retrieving market overview");
            }
        }

        /// <summary>
        /// Get all sectors
        /// </summary>
        [HttpGet("sectors")]
        public async Task<ActionResult<List<string>>> GetSectors()
        {
            try
            {
                var sectors = await _stockService.GetSectorsAsync();
                return Ok(sectors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving sectors");
                return StatusCode(500, "An error occurred while retrieving sectors");
            }
        }

        /// <summary>
        /// Get all exchanges
        /// </summary>
        [HttpGet("exchanges")]
        public async Task<ActionResult<List<string>>> GetExchanges()
        {
            try
            {
                var exchanges = await _stockService.GetExchangesAsync();
                return Ok(exchanges);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving exchanges");
                return StatusCode(500, "An error occurred while retrieving exchanges");
            }
        }
    }
}