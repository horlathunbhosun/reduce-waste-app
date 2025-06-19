using Npgsql;
using Npgsql.Replication;
using Npgsql.Replication.PgOutput;
using Npgsql.Replication.PgOutput.Messages;
using System.Text.Json;
using API.Models;
using API.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Services.CDC;

public interface IPostgresWALCDCService
{
    Task StartWALConsumerAsync(CancellationToken cancellationToken = default);
    Task StopWALConsumerAsync();
    Task<WALChangeStats> GetWALStatsAsync();
    Task<List<WALChangeEvent>> GetRecentWALChangesAsync(int limit = 50);
    bool IsRunning { get; }
}

public class PostgresWALCDCService : IPostgresWALCDCService, IDisposable
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PostgresWALCDCService> _logger;
    private readonly List<WALChangeEvent> _recentChanges = new();
    private readonly object _lock = new();
    
    private LogicalReplicationConnection? _replicationConnection;
    private Task? _consumerTask;
    private CancellationTokenSource? _cancellationTokenSource;
    private bool _isRunning = false;

    public bool IsRunning => _isRunning;

    public PostgresWALCDCService(
        IConfiguration configuration,
        IServiceProvider serviceProvider,
        ILogger<PostgresWALCDCService> logger)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartWALConsumerAsync(CancellationToken cancellationToken = default)
    {
        if (_isRunning)
        {
            _logger.LogWarning("WAL Consumer is already running");
            return;
        }

        try
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            
            // Create replication connection
            var connectionString = _configuration.GetConnectionString("PostgreSqlConnection");
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString)
            {
                // Override with replication user credentials
                Username = "cdc_user",
                Password = "cdc_password_2024!",
                // Replication specific settings
                LogParameters = true,
                IncludeErrorDetail = true
            };

            _replicationConnection = new LogicalReplicationConnection(connectionStringBuilder.ToString());
            await _replicationConnection.Open(_cancellationTokenSource.Token);

            _consumerTask = ConsumeWALChangesAsync(_cancellationTokenSource.Token);
            _isRunning = true;

            _logger.LogInformation("WAL Consumer started successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start WAL Consumer");
            throw;
        }
    }

    public async Task StopWALConsumerAsync()
    {
        if (!_isRunning)
        {
            _logger.LogWarning("WAL Consumer is not running");
            return;
        }

        try
        {
            _cancellationTokenSource?.Cancel();
            
            if (_consumerTask != null)
            {
                await _consumerTask;
            }

            if (_replicationConnection != null)
            {
                await _replicationConnection.DisposeAsync();
                _replicationConnection = null;
            }

            _isRunning = false;
            _logger.LogInformation("WAL Consumer stopped successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping WAL Consumer");
            throw;
        }
    }

    private async Task ConsumeWALChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            var slot = new PgOutputReplicationSlot("transactions_cdc_slot");
            var options = new PgOutputReplicationOptions("transactions_changes", 1);

            await foreach (var message in _replicationConnection!.StartReplication(slot, options, cancellationToken))
            {
                await ProcessWALMessage(message, cancellationToken);
                
                // Acknowledge the message
                _replicationConnection.SetReplicationStatus(message.WalEnd);
                await _replicationConnection.SendStatusUpdate(cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("WAL Consumer was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in WAL Consumer");
            throw;
        }
    }

    private async Task ProcessWALMessage(PgOutputReplicationMessage message, CancellationToken cancellationToken)
    {
        try
        {
            var changeEvent = await ConvertToChangeEvent(message);
            if (changeEvent != null)
            {
                // Store in memory for recent changes API
                lock (_lock)
                {
                    _recentChanges.Add(changeEvent);
                    // Keep only last 1000 changes in memory
                    if (_recentChanges.Count > 1000)
                    {
                        _recentChanges.RemoveAt(0);
                    }
                }

                // Persist to audit log
                await PersistChangeEvent(changeEvent, cancellationToken);

                _logger.LogDebug("Processed WAL change: {Operation} on transaction {TransactionId}", 
                    changeEvent.Operation, changeEvent.TransactionId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing WAL message");
        }
    }

    private async Task<WALChangeEvent?> ConvertToChangeEvent(PgOutputReplicationMessage message)
    {
        switch (message)
        {
            case InsertMessage insert when insert.Relation.RelationName == "Transactions":
                return new WALChangeEvent
                {
                    Id = Guid.NewGuid(),
                    LSN = insert.WalStart.ToString(),
                    Operation = "INSERT",
                    TableName = "Transactions",
                    TransactionId = ExtractTransactionId(insert.NewRow),
                    NewValues = ConvertRowToJson(insert.NewRow, insert.Relation),
                    Timestamp = DateTime.UtcNow,
                    Source = "WAL"
                };

            case UpdateMessage update when update.Relation.RelationName == "Transactions":
                return new WALChangeEvent
                {
                    Id = Guid.NewGuid(),
                    LSN = update.WalStart.ToString(),
                    Operation = "UPDATE", 
                    TableName = "Transactions",
                    TransactionId = ExtractTransactionId(update.NewRow),
                    OldValues = update.OldRow != null ? ConvertRowToJson(update.OldRow, update.Relation) : null,
                    NewValues = ConvertRowToJson(update.NewRow, update.Relation),
                    ChangedFields = DetectChangedFields(update.OldRow, update.NewRow, update.Relation),
                    Timestamp = DateTime.UtcNow,
                    Source = "WAL"
                };

            case DeleteMessage delete when delete.Relation.RelationName == "Transactions":
                return new WALChangeEvent
                {
                    Id = Guid.NewGuid(),
                    LSN = delete.WalStart.ToString(),
                    Operation = "DELETE",
                    TableName = "Transactions", 
                    TransactionId = ExtractTransactionId(delete.OldRow),
                    OldValues = ConvertRowToJson(delete.OldRow, delete.Relation),
                    Timestamp = DateTime.UtcNow,
                    Source = "WAL"
                };

            default:
                return null;
        }
    }

    private Guid ExtractTransactionId(ReplicationTuple row)
    {
        try
        {
            // Assuming Id is the first column
            var idValue = row[0].Get<Guid>();
            return idValue;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to extract transaction ID from row");
            return Guid.Empty;
        }
    }

    private Dictionary<string, object?> ConvertRowToJson(ReplicationTuple row, RelationMessage relation)
    {
        var result = new Dictionary<string, object?>();
        
        for (int i = 0; i < relation.Columns.Count && i < row.ColumnCount; i++)
        {
            var column = relation.Columns[i];
            var value = row[i].IsNull ? null : row[i].Get<object>();
            result[column.ColumnName] = value;
        }

        return result;
    }

    private List<string> DetectChangedFields(ReplicationTuple? oldRow, ReplicationTuple newRow, RelationMessage relation)
    {
        var changedFields = new List<string>();
        
        if (oldRow == null) return changedFields;

        for (int i = 0; i < relation.Columns.Count && i < newRow.ColumnCount; i++)
        {
            var column = relation.Columns[i];
            var oldValue = oldRow[i].IsNull ? null : oldRow[i].Get<object>();
            var newValue = newRow[i].IsNull ? null : newRow[i].Get<object>();

            if (!Equals(oldValue, newValue))
            {
                changedFields.Add(column.ColumnName);
            }
        }

        return changedFields;
    }

    private async Task PersistChangeEvent(WALChangeEvent changeEvent, CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Convert to TransactionAuditLog
            var auditLog = new TransactionAuditLog
            {
                Id = changeEvent.Id,
                TransactionId = changeEvent.TransactionId,
                Operation = changeEvent.Operation,
                Timestamp = changeEvent.Timestamp,
                ChangeSource = "WAL_LOGICAL_REPLICATION",
                ApplicationUser = "SYSTEM"
            };

            if (changeEvent.OldValues != null)
                auditLog.SetOldValues(changeEvent.OldValues);
            
            if (changeEvent.NewValues != null)
                auditLog.SetNewValues(changeEvent.NewValues);
            
            if (changeEvent.ChangedFields != null)
                auditLog.SetChangedFields(changeEvent.ChangedFields);

            context.TransactionAuditLogs.Add(auditLog);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to persist WAL change event");
        }
    }

    public async Task<WALChangeStats> GetWALStatsAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var stats = new WALChangeStats
            {
                IsConsumerRunning = _isRunning,
                TotalWALChanges = await context.TransactionAuditLogs
                    .CountAsync(x => x.ChangeSource == "WAL_LOGICAL_REPLICATION"),
                ChangesLastHour = await context.TransactionAuditLogs
                    .CountAsync(x => x.ChangeSource == "WAL_LOGICAL_REPLICATION" && 
                                x.Timestamp > DateTime.UtcNow.AddHours(-1)),
                ChangesLastDay = await context.TransactionAuditLogs
                    .CountAsync(x => x.ChangeSource == "WAL_LOGICAL_REPLICATION" && 
                                x.Timestamp > DateTime.UtcNow.AddDays(-1)),
                LastChangeTimestamp = await context.TransactionAuditLogs
                    .Where(x => x.ChangeSource == "WAL_LOGICAL_REPLICATION")
                    .MaxAsync(x => (DateTime?)x.Timestamp)
            };

            lock (_lock)
            {
                stats.RecentChangesCount = _recentChanges.Count;
            }

            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get WAL stats");
            return new WALChangeStats { IsConsumerRunning = _isRunning };
        }
    }

    public Task<List<WALChangeEvent>> GetRecentWALChangesAsync(int limit = 50)
    {
        lock (_lock)
        {
            return Task.FromResult(_recentChanges
                .OrderByDescending(x => x.Timestamp)
                .Take(limit)
                .ToList());
        }
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Cancel();
        _replicationConnection?.Dispose();
        _cancellationTokenSource?.Dispose();
    }
}

public class WALChangeEvent
{
    public Guid Id { get; set; }
    public string LSN { get; set; } = string.Empty;
    public string Operation { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public Guid TransactionId { get; set; }
    public Dictionary<string, object?>? OldValues { get; set; }
    public Dictionary<string, object?>? NewValues { get; set; }
    public List<string>? ChangedFields { get; set; }
    public DateTime Timestamp { get; set; }
    public string Source { get; set; } = string.Empty;
}

public class WALChangeStats
{
    public bool IsConsumerRunning { get; set; }
    public int TotalWALChanges { get; set; }
    public int ChangesLastHour { get; set; }
    public int ChangesLastDay { get; set; }
    public int RecentChangesCount { get; set; }
    public DateTime? LastChangeTimestamp { get; set; }
} 