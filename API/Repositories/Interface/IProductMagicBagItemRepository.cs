using API.Models;

namespace API.Repositories.Interface;

public interface IProductMagicBagItemRepository
{
    Task<ProductMagicBagItem> CreateProductMagicBagItem(ProductMagicBagItem productMagicBagItem);
    
    Task<ProductMagicBagItem> UpdateProductMagicBagItem(ProductMagicBagItem productMagicBagItem);
    
    Task<List<ProductMagicBagItem>> GetProductMagicBagItems(Guid magicBagId);

    Task<ProductMagicBagItem?> FindProductMagicItemByProductIdAndMagicBagItem(Guid productId, Guid magicBagId);


}