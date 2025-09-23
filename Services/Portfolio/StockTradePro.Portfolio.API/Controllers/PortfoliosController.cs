// ===== Controllers/PortfoliosController.cs =====
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockTradePro.Portfolio.API.Models.DTOs;
using StockTradePro.Portfolio.API.Services;
using System.Security.Claims;

namespace StockTradePro.Portfolio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PortfoliosController : ControllerBase
    {
        private readonly IPortfolioService _portfolioService;
        private readonly ILogger<PortfoliosController> _logger;

        public PortfoliosController(IPortfolioService portfolioService, ILogger<PortfoliosController> logger)
        {
            _portfolioService = portfolioService;
            _logger = logger;
        }

        /// <summary>
        /// Get user's portfolios with pagination
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<PortfolioSummaryDto>>> GetPortfolios(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("User ID not found in token");

                if (pageSize > 50) pageSize = 50; // Limit page size

                var result = await _portfolioService.GetUserPortfoliosAsync(userId, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user portfolios");
                return StatusCode(500, "An error occurred while retrieving portfolios");
            }
        }

        /// <summary>
        /// Get portfolio by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<PortfolioDto>> GetPortfolio(int id)
        {
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("User ID not found in token");

                var portfolio = await _portfolioService.GetPortfolioByIdAsync(id, userId);
                if (portfolio == null)
                    return NotFound($"Portfolio with ID {id} not found");

                return Ok(portfolio);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving portfolio with ID: {PortfolioId}", id);
                return StatusCode(500, "An error occurred while retrieving the portfolio");
            }
        }

        /// <summary>
        /// Create new portfolio
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PortfolioDto>> CreatePortfolio([FromBody] CreatePortfolioDto createPortfolioDto)
        {
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("User ID not found in token");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var portfolio = await _portfolioService.CreatePortfolioAsync(userId, createPortfolioDto);
                return CreatedAtAction(nameof(GetPortfolio), new { id = portfolio.Id }, portfolio);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating portfolio");
                return StatusCode(500, "An error occurred while creating the portfolio");
            }
        }

        /// <summary>
        /// Update portfolio
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<PortfolioDto>> UpdatePortfolio(int id, [FromBody] UpdatePortfolioDto updatePortfolioDto)
        {
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("User ID not found in token");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var portfolio = await _portfolioService.UpdatePortfolioAsync(id, userId, updatePortfolioDto);
                if (portfolio == null)
                    return NotFound($"Portfolio with ID {id} not found");

                return Ok(portfolio);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating portfolio with ID: {PortfolioId}", id);
                return StatusCode(500, "An error occurred while updating the portfolio");
            }
        }

        /// <summary>
        /// Delete portfolio
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePortfolio(int id)
        {
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("User ID not found in token");

                var result = await _portfolioService.DeletePortfolioAsync(id, userId);
                if (!result)
                    return NotFound($"Portfolio with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting portfolio with ID: {PortfolioId}", id);
                return StatusCode(500, "An error occurred while deleting the portfolio");
            }
        }

        /// <summary>
        /// Get public portfolios
        /// </summary>
        [HttpGet("public")]
        [AllowAnonymous]
        public async Task<ActionResult<List<PortfolioSummaryDto>>> GetPublicPortfolios(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (pageSize > 50) pageSize = 50;

                var portfolios = await _portfolioService.GetPublicPortfoliosAsync(page, pageSize);
                return Ok(portfolios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving public portfolios");
                return StatusCode(500, "An error occurred while retrieving public portfolios");
            }
        }

        /// <summary>
        /// Recalculate portfolio value
        /// </summary>
        [HttpPost("{id}/recalculate")]
        public async Task<ActionResult> RecalculatePortfolio(int id)
        {
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("User ID not found in token");

                var exists = await _portfolioService.PortfolioExistsAsync(id, userId);
                if (!exists)
                    return NotFound($"Portfolio with ID {id} not found");

                await _portfolioService.RecalculatePortfolioValueAsync(id);
                return Ok(new { message = "Portfolio value recalculated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recalculating portfolio with ID: {PortfolioId}", id);
                return StatusCode(500, "An error occurred while recalculating the portfolio");
            }
        }

        private string? GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}

