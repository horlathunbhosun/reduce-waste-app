using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace API.Models;

[Table("TransactionAuditLogs")]
public class TransactionAuditLog
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid TransactionId { get; set; }

    [Required]
    public string Operation { get; set; } = string.Empty; // INSERT, UPDATE, DELETE

    [Required]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public string? UserId { get; set; }

    // Store the old values (before change) as JSON
    [Column(TypeName = "jsonb")]
    public string? OldValues { get; set; }

    // Store the new values (after change) as JSON
    [Column(TypeName = "jsonb")]
    public string? NewValues { get; set; }

    // Store the specific fields that changed
    [Column(TypeName = "jsonb")]
    public string? ChangedFields { get; set; }

    public string? ApplicationUser { get; set; } // Who made the change (if available)

    public string? ChangeSource { get; set; } = "APPLICATION"; // APPLICATION, DIRECT_DB, etc.

    // Navigation property
    public virtual Transactions? Transaction { get; set; }

    // Helper methods for JSON serialization/deserialization
    public Dictionary<string, object?> GetOldValues()
    {
        if (string.IsNullOrEmpty(OldValues))
            return new Dictionary<string, object?>();
            
        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, object?>>(OldValues) ?? new Dictionary<string, object?>();
        }
        catch
        {
            return new Dictionary<string, object?>();
        }
    }

    public Dictionary<string, object?> GetNewValues()
    {
        if (string.IsNullOrEmpty(NewValues))
            return new Dictionary<string, object?>();
            
        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, object?>>(NewValues) ?? new Dictionary<string, object?>();
        }
        catch
        {
            return new Dictionary<string, object?>();
        }
    }

    public List<string> GetChangedFields()
    {
        if (string.IsNullOrEmpty(ChangedFields))
            return new List<string>();
            
        try
        {
            return JsonSerializer.Deserialize<List<string>>(ChangedFields) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    public void SetOldValues(Dictionary<string, object?> values)
    {
        OldValues = JsonSerializer.Serialize(values);
    }

    public void SetNewValues(Dictionary<string, object?> values)
    {
        NewValues = JsonSerializer.Serialize(values);
    }

    public void SetChangedFields(List<string> fields)
    {
        ChangedFields = JsonSerializer.Serialize(fields);
    }
} 