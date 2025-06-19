namespace API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

[Table("Transactions")]
public class Transactions
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey("Users")]
    public string? UserId { get; set; }

    public virtual Users? Users { get; set; }

    [ForeignKey("MagicBag")]
    public Guid? MagicBagId { get; set; }

    public virtual MagicBag? MagicBag { get; set; }

    public DateTime TransactionDate { get; set; }

    [Column(TypeName = "double(12,2)")]
    public double Amount { get; set; }
    
    public string? Status { get; set; } = "Pending";

    public DateTime PickUpdateDate { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    // Dynamic columns stored as JSONB
    [Column(TypeName = "jsonb")]
    public string? DynamicColumns { get; set; } = "{}";
    
    // Helper method to get dynamic column data
    public Dictionary<string, object?> GetDynamicColumnData()
    {
        if (string.IsNullOrEmpty(DynamicColumns))
            return new Dictionary<string, object?>();
            
        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, object?>>(DynamicColumns) ?? new Dictionary<string, object?>();
        }
        catch
        {
            return new Dictionary<string, object?>();
        }
    }
    
    // Helper method to set dynamic column data
    public void SetDynamicColumnData(Dictionary<string, object?> data)
    {
        DynamicColumns = JsonSerializer.Serialize(data);
        UpdatedAt = DateTime.UtcNow;
    }
    
    // Helper method to add or update a single dynamic column
    public void SetDynamicColumn(string columnName, object? value)
    {
        var data = GetDynamicColumnData();
        data[columnName] = value;
        SetDynamicColumnData(data);
    }
    
    // Helper method to get a single dynamic column value
    public T? GetDynamicColumn<T>(string columnName)
    {
        var data = GetDynamicColumnData();
        if (data.TryGetValue(columnName, out var value) && value != null)
        {
            try
            {
                if (value is JsonElement jsonElement)
                {
                    return JsonSerializer.Deserialize<T>(jsonElement.GetRawText());
                }
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return default(T);
            }
        }
        return default(T);
    }
}