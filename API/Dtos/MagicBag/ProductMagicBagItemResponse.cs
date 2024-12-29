using API.Dtos.Product;

namespace API.Dtos.MagicBag;

public class ProductMagicBagItemResponse
{
    public Guid Id { get; set; }

    

    public  MagicBagResponseDto? MagicBag { get; set; }
    
    public  ProductResponseDto? Products { get; set; }
    
    public int Quantity { get; set; }
    
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
    
    
    

}