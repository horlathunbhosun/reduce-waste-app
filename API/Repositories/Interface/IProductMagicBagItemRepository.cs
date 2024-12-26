using API.Models;

namespace API.Repositories.Interface;

public interface IProductMagicBagItemRepository
{
    Task<ProductMagicBagItem> CreateProductMagicBagItem(ProductMagicBagItem productMagicBagItem);
    
    Task<ProductMagicBagItem> UpdateProductMagicBagItem(ProductMagicBagItem productMagicBagItem);
    
}