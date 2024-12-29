namespace API.Dtos.MagicBag;

public class MagicBagRequestDto
{
    public string? Name { get; set; }
    
    
    public string? Description { get; set; }

    public double BagPrice { get; set; }
    

    public  int PartnerId { get; set; }
    
    // public  List<ProductMagicBagItemRequest>? MagicBagItems { get; set; }
}