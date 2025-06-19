using API.Models;

namespace API.Dtos.Response;

public class TransactionAuditLogDto
{
    public Guid Id { get; set; }
    public Guid TransactionId { get; set; }
    public string Operation { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? UserId { get; set; }
    public Dictionary<string, object?>? OldValues { get; set; }
    public Dictionary<string, object?>? NewValues { get; set; }
    public List<string>? ChangedFields { get; set; }
    public string? ApplicationUser { get; set; }
    public string? ChangeSource { get; set; }
    public TransactionSummaryDto? Transaction { get; set; }
}

public class TransactionSummaryDto
{
    public Guid Id { get; set; }
    public string? UserId { get; set; }
    public Guid? MagicBagId { get; set; }
    public DateTime TransactionDate { get; set; }
    public double Amount { get; set; }
    public string? Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class TransactionChangesResponse
{
    public int TotalCount { get; set; }
    public List<TransactionAuditLogDto> Changes { get; set; } = new();
}

public class TransactionChangeAnalytics
{
    public int TotalChanges { get; set; }
    public int TotalInserts { get; set; }
    public int TotalUpdates { get; set; }
    public int TotalDeletes { get; set; }
    public Dictionary<string, int> ChangesByUser { get; set; } = new();
    public Dictionary<string, int> ChangesByField { get; set; } = new();
    public Dictionary<DateTime, int> ChangesByDate { get; set; } = new();
} 