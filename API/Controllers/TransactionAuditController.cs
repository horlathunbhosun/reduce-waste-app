using API.Dtos.Response;
using API.Models;
using API.Services.CDC;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionAuditController : ControllerBase
{
    private readonly ITransactionCDCService _cdcService;
    private readonly ILogger<TransactionAuditController> _logger;

    public TransactionAuditController(
        ITransactionCDCService cdcService,
        ILogger<TransactionAuditController> logger)
    {
        _cdcService = cdcService;
        _logger = logger;
    }

    /// <summary>
    /// Get the complete change history for a specific transaction
    /// </summary>
    [HttpGet("transaction/{transactionId}/history")]
    public async Task<ActionResult<GenericResponse>> GetTransactionHistory(Guid transactionId)
    {
        try
        {
            var history = await _cdcService.GetTransactionHistoryAsync(transactionId);
            var response = history.Select(MapToDto).ToList();

            return Ok(GenericResponse.FromSuccess(new TransactionChangesResponse
            {
                TotalCount = response.Count,
                Changes = response
            }, "Transaction history retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transaction history for ID: {TransactionId}", transactionId);
            return StatusCode(500, GenericResponse.FromError(
                new ErrorResponse("Internal Server Error", "An error occurred while retrieving transaction history", 500), 500));
        }
    }

    /// <summary>
    /// Get recent changes across all transactions
    /// </summary>
    [HttpGet("recent")]
    public async Task<ActionResult<GenericResponse>> GetRecentChanges([FromQuery] int limit = 50)
    {
        try
        {
            if (limit > 1000) limit = 1000; // Prevent excessive data retrieval

            var changes = await _cdcService.GetRecentChangesAsync(limit);
            var response = changes.Select(MapToDto).ToList();

            return Ok(GenericResponse.FromSuccess(new TransactionChangesResponse
            {
                TotalCount = response.Count,
                Changes = response
            }, "Recent changes retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving recent changes");
            return StatusCode(500, GenericResponse.FromError(
                new ErrorResponse("Internal Server Error", "An error occurred while retrieving recent changes", 500), 500));
        }
    }

    /// <summary>
    /// Get changes made by a specific user
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<GenericResponse>> GetChangesByUser(string userId, [FromQuery] int limit = 50)
    {
        try
        {
            if (limit > 1000) limit = 1000;

            var changes = await _cdcService.GetChangesByUserAsync(userId, limit);
            var response = changes.Select(MapToDto).ToList();

            return Ok(GenericResponse.FromSuccess(new TransactionChangesResponse
            {
                TotalCount = response.Count,
                Changes = response
            }, "User changes retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving changes for user: {UserId}", userId);
            return StatusCode(500, GenericResponse.FromError(
                new ErrorResponse("Internal Server Error", "An error occurred while retrieving user changes", 500), 500));
        }
    }

    /// <summary>
    /// Get changes within a specific date range
    /// </summary>
    [HttpGet("date-range")]
    public async Task<ActionResult<GenericResponse>> GetChangesByDateRange(
        [FromQuery] DateTime startDate, 
        [FromQuery] DateTime endDate)
    {
        try
        {
            if (endDate <= startDate)
            {
                return BadRequest(GenericResponse.FromError(
                    new ErrorResponse("Invalid Date Range", "End date must be after start date", 400), 400));
            }

            // Limit to 90 days to prevent excessive data retrieval
            if ((endDate - startDate).TotalDays > 90)
            {
                return BadRequest(GenericResponse.FromError(
                    new ErrorResponse("Date Range Too Large", "Date range cannot exceed 90 days", 400), 400));
            }

            var changes = await _cdcService.GetChangesByDateRangeAsync(startDate, endDate);
            var response = changes.Select(MapToDto).ToList();

            return Ok(GenericResponse.FromSuccess(new TransactionChangesResponse
            {
                TotalCount = response.Count,
                Changes = response
            }, "Changes by date range retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving changes for date range: {StartDate} to {EndDate}", startDate, endDate);
            return StatusCode(500, GenericResponse.FromError(
                new ErrorResponse("Internal Server Error", "An error occurred while retrieving changes by date range", 500), 500));
        }
    }

    /// <summary>
    /// Get a specific change by its ID
    /// </summary>
    [HttpGet("{changeId}")]
    public async Task<ActionResult<GenericResponse>> GetChangeById(Guid changeId)
    {
        try
        {
            var change = await _cdcService.GetChangeByIdAsync(changeId);
            
            if (change == null)
            {
                return NotFound(GenericResponse.FromError(
                    new ErrorResponse("Change Not Found", "The specified change was not found", 404), 404));
            }

            var response = MapToDto(change);
            return Ok(GenericResponse.FromSuccess(response, "Change retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving change by ID: {ChangeId}", changeId);
            return StatusCode(500, GenericResponse.FromError(
                new ErrorResponse("Internal Server Error", "An error occurred while retrieving the change", 500), 500));
        }
    }

    /// <summary>
    /// Get analytics about transaction changes
    /// </summary>
    [HttpGet("analytics")]
    public async Task<ActionResult<GenericResponse>> GetChangeAnalytics([FromQuery] int days = 30)
    {
        try
        {
            if (days > 365) days = 365; // Limit to 1 year

            var startDate = DateTime.UtcNow.AddDays(-days);
            var endDate = DateTime.UtcNow;

            var changes = await _cdcService.GetChangesByDateRangeAsync(startDate, endDate);

            var analytics = new TransactionChangeAnalytics
            {
                TotalChanges = changes.Count,
                TotalInserts = changes.Count(c => c.Operation == "INSERT"),
                TotalUpdates = changes.Count(c => c.Operation == "UPDATE"),
                TotalDeletes = changes.Count(c => c.Operation == "DELETE"),
                ChangesByUser = changes
                    .Where(c => !string.IsNullOrEmpty(c.UserId))
                    .GroupBy(c => c.UserId!)
                    .ToDictionary(g => g.Key, g => g.Count()),
                ChangesByField = changes
                    .Where(c => c.Operation == "UPDATE")
                    .SelectMany(c => c.GetChangedFields())
                    .GroupBy(field => field)
                    .ToDictionary(g => g.Key, g => g.Count()),
                ChangesByDate = changes
                    .GroupBy(c => c.Timestamp.Date)
                    .ToDictionary(g => g.Key, g => g.Count())
            };

            return Ok(GenericResponse.FromSuccess(analytics, "Change analytics retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving change analytics for {Days} days", days);
            return StatusCode(500, GenericResponse.FromError(
                new ErrorResponse("Internal Server Error", "An error occurred while retrieving change analytics", 500), 500));
        }
    }

    /// <summary>
    /// Initialize CDC system (Admin only)
    /// </summary>
    [HttpPost("initialize")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<GenericResponse>> InitializeCDC()
    {
        try
        {
            var success = await _cdcService.InitializeCDCAsync();
            
            if (success)
            {
                return Ok(GenericResponse.FromSuccess(true, "CDC system initialized successfully"));
            }
            
            return StatusCode(500, GenericResponse.FromError(
                new ErrorResponse("Initialization Failed", "Failed to initialize CDC system", 500), 500));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing CDC system");
            return StatusCode(500, GenericResponse.FromError(
                new ErrorResponse("Internal Server Error", "An error occurred while initializing CDC system", 500), 500));
        }
    }

    /// <summary>
    /// Setup database triggers (Admin only)
    /// </summary>
    [HttpPost("setup-triggers")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<GenericResponse>> SetupTriggers()
    {
        try
        {
            var success = await _cdcService.SetupDatabaseTriggersAsync();
            
            if (success)
            {
                return Ok(GenericResponse.FromSuccess(true, "Database triggers setup successfully"));
            }
            
            return StatusCode(500, GenericResponse.FromError(
                new ErrorResponse("Setup Failed", "Failed to setup database triggers", 500), 500));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting up database triggers");
            return StatusCode(500, GenericResponse.FromError(
                new ErrorResponse("Internal Server Error", "An error occurred while setting up database triggers", 500), 500));
        }
    }

    /// <summary>
    /// Remove database triggers (Admin only)
    /// </summary>
    [HttpDelete("triggers")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<GenericResponse>> RemoveTriggers()
    {
        try
        {
            var success = await _cdcService.RemoveDatabaseTriggersAsync();
            
            if (success)
            {
                return Ok(GenericResponse.FromSuccess(true, "Database triggers removed successfully"));
            }
            
            return StatusCode(500, GenericResponse.FromError(
                new ErrorResponse("Removal Failed", "Failed to remove database triggers", 500), 500));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing database triggers");
            return StatusCode(500, GenericResponse.FromError(
                new ErrorResponse("Internal Server Error", "An error occurred while removing database triggers", 500), 500));
        }
    }

    private static TransactionAuditLogDto MapToDto(TransactionAuditLog auditLog)
    {
        return new TransactionAuditLogDto
        {
            Id = auditLog.Id,
            TransactionId = auditLog.TransactionId,
            Operation = auditLog.Operation,
            Timestamp = auditLog.Timestamp,
            UserId = auditLog.UserId,
            OldValues = auditLog.GetOldValues(),
            NewValues = auditLog.GetNewValues(),
            ChangedFields = auditLog.GetChangedFields(),
            ApplicationUser = auditLog.ApplicationUser,
            ChangeSource = auditLog.ChangeSource,
            Transaction = auditLog.Transaction != null ? new TransactionSummaryDto
            {
                Id = auditLog.Transaction.Id,
                UserId = auditLog.Transaction.UserId,
                MagicBagId = auditLog.Transaction.MagicBagId,
                TransactionDate = auditLog.Transaction.TransactionDate,
                Amount = auditLog.Transaction.Amount,
                Status = auditLog.Transaction.Status,
                CreatedAt = auditLog.Transaction.CreatedAt,
                UpdatedAt = auditLog.Transaction.UpdatedAt
            } : null
        };
    }
} 