
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using StockTradePro.WatchlistService.API.DTOs;
using StockTradePro.WatchlistService.API.Services;

namespace StockTradePro.WatchlistService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WatchlistsController : ControllerBase
    {
        private readonly IWatchlistService _watchlistService;
        private readonly ILogger<WatchlistsController> _logger;

        public WatchlistsController(IWatchlistService watchlistService, ILogger<WatchlistsController> logger)
        {
            _watchlistService = watchlistService;
            _logger = logger;
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                   User.FindFirst("sub")?.Value ??
                   throw new UnauthorizedAccessException("User ID not found in token");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WatchlistDto>>> GetWatchlists()
        {
            try
            {
                var userId = GetUserId();
                var watchlists = await _watchlistService.GetUserWatchlistsAsync(userId);
                return Ok(watchlists);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting watchlists");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WatchlistDto>> GetWatchlist(int id)
        {
            try
            {
                var userId = GetUserId();
                var watchlist = await _watchlistService.GetWatchlistAsync(id, userId);

                if (watchlist == null)
                    return NotFound($"Watchlist with ID {id} not found");

                return Ok(watchlist);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting watchlist {WatchlistId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<WatchlistDto>> CreateWatchlist(CreateWatchlistDto dto)
        {
            try
            {
                var userId = GetUserId();
                var watchlist = await _watchlistService.CreateWatchlistAsync(dto, userId);
                return CreatedAtAction(nameof(GetWatchlist), new { id = watchlist.Id }, watchlist);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating watchlist");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<WatchlistDto>> UpdateWatchlist(int id, UpdateWatchlistDto dto)
        {
            try
            {
                var userId = GetUserId();
                var watchlist = await _watchlistService.UpdateWatchlistAsync(id, dto, userId);

                if (watchlist == null)
                    return NotFound($"Watchlist with ID {id} not found");

                return Ok(watchlist);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating watchlist {WatchlistId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWatchlist(int id)
        {
            try
            {
                var userId = GetUserId();
                var success = await _watchlistService.DeleteWatchlistAsync(id, userId);

                if (!success)
                    return NotFound($"Watchlist with ID {id} not found");

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting watchlist {WatchlistId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/items")]
        public async Task<ActionResult<IEnumerable<WatchlistItemDto>>> GetWatchlistItems(int id)
        {
            try
            {
                var userId = GetUserId();
                var items = await _watchlistService.GetWatchlistItemsWithPricesAsync(id, userId);
                return Ok(items);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting watchlist items for {WatchlistId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{id}/items")]
        public async Task<ActionResult<WatchlistItemDto>> AddItemToWatchlist(int id, AddWatchlistItemDto dto)
        {
            try
            {
                var userId = GetUserId();
                var item = await _watchlistService.AddItemToWatchlistAsync(id, dto, userId);

                if (item == null)
                    return BadRequest("Unable to add item to watchlist. Check if the stock symbol exists and is not already in the watchlist.");

                return CreatedAtAction(nameof(GetWatchlistItems), new { id }, item);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to watchlist {WatchlistId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}/items/{itemId}")]
        public async Task<IActionResult> RemoveItemFromWatchlist(int id, int itemId)
        {
            try
            {
                var userId = GetUserId();
                var success = await _watchlistService.RemoveItemFromWatchlistAsync(id, itemId, userId);

                if (!success)
                    return NotFound("Item not found in watchlist");

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing item {ItemId} from watchlist {WatchlistId}", itemId, id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPatch("{id}/items/{itemId}/sort-order")]
        public async Task<IActionResult> UpdateItemSortOrder(int id, int itemId, [FromBody] int newSortOrder)
        {
            try
            {
                var userId = GetUserId();
                var success = await _watchlistService.UpdateItemSortOrderAsync(id, itemId, newSortOrder, userId);

                if (!success)
                    return NotFound("Item not found in watchlist");

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating sort order for item {ItemId} in watchlist {WatchlistId}", itemId, id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}