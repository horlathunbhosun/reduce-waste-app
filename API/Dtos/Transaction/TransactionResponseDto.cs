using API.Dtos.MagicBag;
using API.Dtos.User;
using API.Models;

namespace API.Dtos.Transaction;

public class TransactionResponseDto
{
    public Guid Id { get; set; }
    
    public double Amount { get; set; }

    public string? Status { get; set; }
    public virtual UserResponseDto Users { get; set; }
    
    public virtual MagicBagResponseDto MagicBag { get; set; }

    public DateTime TransactionDate { get; set; }
    
    public DateTime PickUpdateDate { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
}
