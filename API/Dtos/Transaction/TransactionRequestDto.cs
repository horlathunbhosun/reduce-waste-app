namespace API.Dtos.Transaction;

public class TransactionRequestDto
{
    public string? UserId { get; set; }
    
    public double Amount { get; set; }
    
    public Guid MagicBagId { get; set; }
    
    
    
}