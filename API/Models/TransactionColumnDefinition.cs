using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

[Table("TransactionColumnDefinitions")]
public class TransactionColumnDefinition
{
    [Key]
    public Guid Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string DisplayName { get; set; } = string.Empty;
    
    public string Type { get; set; } = "Text"; // Text, Choice, DateAndTime, MultipleLines, Person, Number, YesNo
    
    public bool Required { get; set; } = false;
    
    public string? DefaultValue { get; set; }
    
    // Store options as JSON (for Choice type, validation rules, etc.)
    [Column(TypeName = "jsonb")]
    public string? Options { get; set; } = "{}";
    
    public int DisplayOrder { get; set; } = 0;
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
} 