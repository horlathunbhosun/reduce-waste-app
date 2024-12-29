using API.Dtos.User;
using API.Models;

namespace API.Dtos.MagicBag;

public class MagicBagResponseDto
{
    public Guid Id { get; set; }
    
    public string? Name { get; set; }
    
    public string? Description { get; set; }

    public double BagPrice { get; set; }
    
    public PartnerResponseDto? Partner { get; set; }
    
    public  List<ProductMagicBagItemResponse>? MagicBagItems { get; set; }
    
    public string? Status { get; set; }
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}