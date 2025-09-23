
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using StockTradePro.WatchlistService.API.DTOs;
using StockTradePro.WatchlistService.API.Services;

namespace StockTradePro.WatchlistService.API.Controllers
{
    [ApiController]
    [Route("api/watchlists/{watchlistId}/[controller]")]
    [Authorize]
    public class PriceAlertsController : ControllerBase
    {
        private readonly IPriceAlertService _priceAlertService;
        private readonly ILogger<PriceAlertsController> _logger;

        public PriceAlertsController(IPriceAlertService priceAlertService, ILogger<PriceAlertsController> logger)
        {
            _priceAlertService = priceAlertService;
            _logger = logger;
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                   User.FindFirst("sub")?.Value ??
                   throw new UnauthorizedAccessException("User ID not found in token");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PriceAlertDto>>> GetPriceAlerts(int watchlistId)
        {
            try
            {
                var userId = GetUserId();
                var alerts = await _priceAlertService.GetWatchlistAlertsAsync(watchlistId, userId);
                return Ok(alerts);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting price alerts for watchlist {WatchlistId}", watchlistId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<PriceAlertDto>> CreatePriceAlert(int watchlistId, CreatePriceAlertDto dto)
        {
            try
            {
                var userId = GetUserId();
                var alert = await _priceAlertService.CreateAlertAsync(watchlistId, dto, userId);

                if (alert == null)
                    return BadRequest("Unable to create price alert. Check if the watchlist exists and symbol is valid.");

                return CreatedAtAction(nameof(GetPriceAlert), new { watchlistId, id = alert.Id }, alert);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating price alert for watchlist {WatchlistId}", watchlistId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PriceAlertDto>> GetPriceAlert(int watchlistId, int id)
        {
            try
            {
                var userId = GetUserId();
                var alert = await _priceAlertService.GetAlertAsync(id, userId);

                if (alert == null || alert.WatchlistId != watchlistId)
                    return NotFound($"Price alert with ID {id} not found");

                return Ok(alert);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting price alert {AlertId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePriceAlert(int watchlistId, int id)
        {
            try
            {
                var userId = GetUserId();
                var success = await _priceAlertService.DeleteAlertAsync(id, userId);

                if (!success)
                    return NotFound($"Price alert with ID {id} not found");

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting price alert {AlertId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPatch("{id}/disable")]
        public async Task<IActionResult> DisablePriceAlert(int watchlistId, int id)
        {
            try
            {
                var userId = GetUserId();
                var success = await _priceAlertService.DisableAlertAsync(id, userId);

                if (!success)
                    return NotFound($"Price alert with ID {id} not found");

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disabling price alert {AlertId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}