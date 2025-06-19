using API.Dtos.Response;
using API.Services.CDC;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WALCDCController : ControllerBase
{
    private readonly IPostgresWALCDCService _walCdcService;
    private readonly ILogger<WALCDCController> _logger;

    public WALCDCController(
        IPostgresWALCDCService walCdcService,
        ILogger<WALCDCController> logger)
    {
        _walCdcService = walCdcService;
        _logger = logger;
    }

    /// <summary>
    /// Start the WAL CDC consumer
    /// </summary>
    [HttpPost("start")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<GenericResponse>> StartWALConsumer()
    {
        try
        {
            if (_walCdcService.IsRunning)
            {
                return BadRequest(GenericResponse.FromError(
                    new ErrorResponse("Already Running", "WAL CDC consumer is already running", 400), 400));
            }

            await _walCdcService.StartWALConsumerAsync();
            
            return Ok(GenericResponse.FromSuccess(true, "WAL CDC consumer started successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting WAL CDC consumer");
            return StatusCode(500, GenericResponse.FromError(
                new ErrorResponse("Internal Server Error", "Failed to start WAL CDC consumer", 500), 500));
        }
    }

    /// <summary>
    /// Stop the WAL CDC consumer
    /// </summary>
    [HttpPost("stop")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<GenericResponse>> StopWALConsumer()
    {
        try
        {
            if (!_walCdcService.IsRunning)
            {
                return BadRequest(GenericResponse.FromError(
                    new ErrorResponse("Not Running", "WAL CDC consumer is not running", 400), 400));
            }

            await _walCdcService.StopWALConsumerAsync();
            
            return Ok(GenericResponse.FromSuccess(true, "WAL CDC consumer stopped successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping WAL CDC consumer");
            return StatusCode(500, GenericResponse.FromError(
                new ErrorResponse("Internal Server Error", "Failed to stop WAL CDC consumer", 500), 500));
        }
    }

    /// <summary>
    /// Get WAL CDC consumer status and statistics
    /// </summary>
    [HttpGet("status")]
    public async Task<ActionResult<GenericResponse>> GetWALConsumerStatus()
    {
        try
        {
            var stats = await _walCdcService.GetWALStatsAsync();
            
            return Ok(GenericResponse.FromSuccess(stats, "WAL CDC status retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving WAL CDC status");
            return StatusCode(500, GenericResponse.FromError(
                new ErrorResponse("Internal Server Error", "Failed to retrieve WAL CDC status", 500), 500));
        }
    }

    /// <summary>
    /// Get recent WAL changes (from memory buffer)
    /// </summary>
    [HttpGet("recent-changes")]
    public async Task<ActionResult<GenericResponse>> GetRecentWALChanges([FromQuery] int limit = 50)
    {
        try
        {
            if (limit > 500) limit = 500; // Prevent excessive data retrieval

            var changes = await _walCdcService.GetRecentWALChangesAsync(limit);
            
            return Ok(GenericResponse.FromSuccess(changes, "Recent WAL changes retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving recent WAL changes");
            return StatusCode(500, GenericResponse.FromError(
                new ErrorResponse("Internal Server Error", "Failed to retrieve recent WAL changes", 500), 500));
        }
    }

    /// <summary>
    /// Get real-time WAL change stream (Server-Sent Events)
    /// </summary>
    [HttpGet("stream")]
    public async Task GetWALChangeStream()
    {
        try
        {
            Response.Headers.Add("Content-Type", "text/event-stream");
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("Connection", "keep-alive");
            Response.Headers.Add("Access-Control-Allow-Origin", "*");

            var cancellationToken = HttpContext.RequestAborted;
            var lastChangeTime = DateTime.UtcNow;

            await Response.WriteAsync("data: Connected to WAL change stream\n\n");
            await Response.Body.FlushAsync();

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var changes = await _walCdcService.GetRecentWALChangesAsync(10);
                    var newChanges = changes.Where(c => c.Timestamp > lastChangeTime).ToList();

                    if (newChanges.Any())
                    {
                        foreach (var change in newChanges)
                        {
                            var eventData = System.Text.Json.JsonSerializer.Serialize(new
                            {
                                id = change.Id,
                                operation = change.Operation,
                                transactionId = change.TransactionId,
                                timestamp = change.Timestamp,
                                lsn = change.LSN,
                                changedFields = change.ChangedFields
                            });

                            await Response.WriteAsync($"data: {eventData}\n\n");
                            await Response.Body.FlushAsync();
                        }

                        lastChangeTime = newChanges.Max(c => c.Timestamp);
                    }

                    await Task.Delay(1000, cancellationToken); // Poll every second
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in WAL change stream");
                    await Response.WriteAsync($"data: Error: {ex.Message}\n\n");
                    await Response.Body.FlushAsync();
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting up WAL change stream");
        }
    }

    /// <summary>
    /// Get WAL replication lag and health metrics
    /// </summary>
    [HttpGet("health")]
    public async Task<ActionResult<GenericResponse>> GetWALHealth()
    {
        try
        {
            var stats = await _walCdcService.GetWALStatsAsync();
            
            var health = new
            {
                IsHealthy = _walCdcService.IsRunning && stats.LastChangeTimestamp.HasValue && 
                           stats.LastChangeTimestamp.Value > DateTime.UtcNow.AddMinutes(-5),
                ConsumerRunning = _walCdcService.IsRunning,
                LastChangeAge = stats.LastChangeTimestamp.HasValue ? 
                    DateTime.UtcNow - stats.LastChangeTimestamp.Value : (TimeSpan?)null,
                ChangesPerHour = stats.ChangesLastHour,
                ChangesPerDay = stats.ChangesLastDay,
                MemoryBufferSize = stats.RecentChangesCount,
                Recommendations = GetHealthRecommendations(stats)
            };
            
            return Ok(GenericResponse.FromSuccess(health, "WAL health metrics retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving WAL health metrics");
            return StatusCode(500, GenericResponse.FromError(
                new ErrorResponse("Internal Server Error", "Failed to retrieve WAL health metrics", 500), 500));
        }
    }

    /// <summary>
    /// Test WAL CDC connectivity and configuration
    /// </summary>
    [HttpPost("test")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<GenericResponse>> TestWALConfiguration()
    {
        try
        {
            var testResults = new List<object>();

            // Test 1: Check if consumer is running
            testResults.Add(new
            {
                Test = "Consumer Status",
                Status = _walCdcService.IsRunning ? "PASS" : "FAIL",
                Message = _walCdcService.IsRunning ? "Consumer is running" : "Consumer is not running"
            });

            // Test 2: Check recent activity
            var stats = await _walCdcService.GetWALStatsAsync();
            testResults.Add(new
            {
                Test = "Recent Activity",
                Status = stats.ChangesLastHour > 0 ? "PASS" : "WARNING",
                Message = $"Processed {stats.ChangesLastHour} changes in the last hour"
            });

            // Test 3: Check memory buffer
            testResults.Add(new
            {
                Test = "Memory Buffer",
                Status = stats.RecentChangesCount > 0 ? "PASS" : "INFO",
                Message = $"Memory buffer contains {stats.RecentChangesCount} recent changes"
            });

            // Test 4: Check last change timestamp
            var lastChangeStatus = "UNKNOWN";
            var lastChangeMessage = "No changes recorded";
            
            if (stats.LastChangeTimestamp.HasValue)
            {
                var age = DateTime.UtcNow - stats.LastChangeTimestamp.Value;
                if (age.TotalMinutes < 5)
                {
                    lastChangeStatus = "PASS";
                    lastChangeMessage = $"Last change was {age.TotalMinutes:F1} minutes ago";
                }
                else if (age.TotalHours < 1)
                {
                    lastChangeStatus = "WARNING";
                    lastChangeMessage = $"Last change was {age.TotalMinutes:F0} minutes ago";
                }
                else
                {
                    lastChangeStatus = "FAIL";
                    lastChangeMessage = $"Last change was {age.TotalHours:F1} hours ago";
                }
            }

            testResults.Add(new
            {
                Test = "Last Change Timestamp",
                Status = lastChangeStatus,
                Message = lastChangeMessage
            });

            var overallStatus = testResults.All(t => t.GetType().GetProperty("Status")?.GetValue(t)?.ToString() == "PASS") ? "HEALTHY" :
                               testResults.Any(t => t.GetType().GetProperty("Status")?.GetValue(t)?.ToString() == "FAIL") ? "UNHEALTHY" : "WARNING";

            var result = new
            {
                OverallStatus = overallStatus,
                TestResults = testResults,
                Statistics = stats,
                TestedAt = DateTime.UtcNow
            };
            
            return Ok(GenericResponse.FromSuccess(result, "WAL configuration test completed"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing WAL configuration");
            return StatusCode(500, GenericResponse.FromError(
                new ErrorResponse("Internal Server Error", "Failed to test WAL configuration", 500), 500));
        }
    }

    private List<string> GetHealthRecommendations(WALChangeStats stats)
    {
        var recommendations = new List<string>();

        if (!stats.IsConsumerRunning)
        {
            recommendations.Add("Start the WAL CDC consumer to begin processing changes");
        }

        if (stats.ChangesLastHour == 0 && stats.IsConsumerRunning)
        {
            recommendations.Add("No changes detected in the last hour - verify database activity");
        }

        if (stats.ChangesLastDay > 10000)
        {
            recommendations.Add("High change volume detected - consider implementing data archiving");
        }

        if (stats.RecentChangesCount > 800)
        {
            recommendations.Add("Memory buffer is getting full - consider increasing processing frequency");
        }

        if (stats.LastChangeTimestamp.HasValue && DateTime.UtcNow - stats.LastChangeTimestamp.Value > TimeSpan.FromHours(1))
        {
            recommendations.Add("No recent changes - check PostgreSQL WAL configuration and replication slot");
        }

        if (recommendations.Count == 0)
        {
            recommendations.Add("WAL CDC system is operating normally");
        }

        return recommendations;
    }
} 