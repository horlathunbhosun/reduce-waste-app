namespace API.Dtos;

public class AddTransactionColumnDto
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Type { get; set; } = "Text"; // Text, Choice, DateAndTime, MultipleLines, Person, Number, YesNo
    public bool Required { get; set; } = false;
    public string? DefaultValue { get; set; }
    public Dictionary<string, object> Options { get; set; } = new();
    public int DisplayOrder { get; set; } = 0;
}

public class UpdateTransactionColumnDto
{
    public string DisplayName { get; set; } = string.Empty;
    public bool Required { get; set; } = false;
    public string? DefaultValue { get; set; }
    public Dictionary<string, object> Options { get; set; } = new();
    public int DisplayOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}

public class TransactionColumnDefinitionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool Required { get; set; } = false;
    public string? DefaultValue { get; set; }
    public Dictionary<string, object> Options { get; set; } = new();
    public int DisplayOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class TransactionWithDynamicColumnsDto
{
    public Guid Id { get; set; }
    public string? UserId { get; set; }
    public Guid? MagicBagId { get; set; }
    public DateTime TransactionDate { get; set; }
    public double Amount { get; set; }
    public string? Status { get; set; }
    public DateTime PickUpdateDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Dynamic column data
    public Dictionary<string, object?> DynamicColumnData { get; set; } = new();
}

public class UpdateTransactionWithDynamicColumnsDto
{
    public string? Status { get; set; }
    public DateTime? PickUpdateDate { get; set; }
    public Dictionary<string, object?> DynamicColumnData { get; set; } = new();
}

public class CreateTransactionWithDynamicColumnsDto
{
    public string? UserId { get; set; }
    public Guid? MagicBagId { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    public double Amount { get; set; }
    public string? Status { get; set; } = "Pending";
    public DateTime PickUpdateDate { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object?> DynamicColumnData { get; set; } = new();
} 