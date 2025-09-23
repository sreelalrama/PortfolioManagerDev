// ===== Controllers/TransactionsController.cs =====
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
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(ITransactionService transactionService, ILogger<TransactionsController> logger)
        {
            _transactionService = transactionService;
            _logger = logger;
        }

        /// <summary>
        /// Get transactions for a portfolio
        /// </summary>
        [HttpGet("portfolio/{portfolioId}")]
        public async Task<ActionResult<PaginatedResult<TransactionDto>>> GetPortfolioTransactions(
            int portfolioId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("User ID not found in token");

                if (pageSize > 100) pageSize = 100;

                var result = await _transactionService.GetPortfolioTransactionsAsync(portfolioId, userId, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving transactions for portfolio: {PortfolioId}", portfolioId);
                return StatusCode(500, "An error occurred while retrieving transactions");
            }
        }

        /// <summary>
        /// Get transaction by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionDto>> GetTransaction(int id)
        {
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("User ID not found in token");

                var transaction = await _transactionService.GetTransactionByIdAsync(id, userId);
                if (transaction == null)
                    return NotFound($"Transaction with ID {id} not found");

                return Ok(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving transaction with ID: {TransactionId}", id);
                return StatusCode(500, "An error occurred while retrieving the transaction");
            }
        }

        /// <summary>
        /// Create new transaction
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TransactionDto>> CreateTransaction([FromBody] CreateTransactionDto createTransactionDto)
        {
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("User ID not found in token");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var transaction = await _transactionService.CreateTransactionAsync(userId, createTransactionDto);
                return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating transaction");
                return StatusCode(500, "An error occurred while creating the transaction");
            }
        }

        /// <summary>
        /// Delete transaction
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTransaction(int id)
        {
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("User ID not found in token");

                var result = await _transactionService.DeleteTransactionAsync(id, userId);
                if (!result)
                    return NotFound($"Transaction with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting transaction with ID: {TransactionId}", id);
                return StatusCode(500, "An error occurred while deleting the transaction");
            }
        }

        /// <summary>
        /// Get transactions by symbol
        /// </summary>
        [HttpGet("portfolio/{portfolioId}/symbol/{symbol}")]
        public async Task<ActionResult<List<TransactionDto>>> GetTransactionsBySymbol(int portfolioId, string symbol)
        {
            try
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("User ID not found in token");

                var transactions = await _transactionService.GetTransactionsBySymbolAsync(portfolioId, symbol, userId);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving transactions for portfolio: {PortfolioId}, symbol: {Symbol}", portfolioId, symbol);
                return StatusCode(500, "An error occurred while retrieving transactions");
            }
        }

        private string? GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}