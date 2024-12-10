using API.Models;

namespace API.Dtos.Product;

public class ProductResponseDto
{
   
    public Guid Id { get; set; }
    
    public  string? Name { get; set; }
    
    public string? Description { get; set; }
    
    public  List<ProductMagicBagItem>? MagicBagItems { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}