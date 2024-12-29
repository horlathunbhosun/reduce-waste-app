namespace API.Dtos.MagicBag;

public class ProductMagicBagItemRequest
{
    public Guid? MagicBagId { get; set; }
    
    public Guid? ProductId { get; set; }
    
    public int Quantity { get; set; }

}