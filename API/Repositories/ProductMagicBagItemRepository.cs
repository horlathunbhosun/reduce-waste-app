using API.Data;
using API.Models;
using API.Repositories.Interface;

namespace API.Repositories;

public class ProductMagicBagItemRepository : IProductMagicBagItemRepository
{
    private readonly ApplicationDbContext _context;
    
    public async Task<ProductMagicBagItem?> GetProductMagicBagItemById(Guid id)
    {
        return await _context.ProductMagicBagItems.FindAsync(id);   
    }

  

    public async Task<ProductMagicBagItem> CreateProductMagicBagItem(ProductMagicBagItem productMagicBagItem)
    {
        await _context.ProductMagicBagItems.AddAsync(productMagicBagItem);
        await _context.SaveChangesAsync();
        Console.WriteLine($"Product Magic Bag Item Created Successfully: {productMagicBagItem.Id}");
        return productMagicBagItem;
    }

    public async Task<ProductMagicBagItem> UpdateProductMagicBagItem(ProductMagicBagItem productMagicBagItem)
    {
        var productMagicBagItemExist = await _context.ProductMagicBagItems.FindAsync(productMagicBagItem.Id);
        if (productMagicBagItemExist == null)
        {
            throw new InvalidOperationException("Product Magic Bag Item does not exist");
        }
        
        _context.Entry(productMagicBagItemExist).CurrentValues.SetValues(productMagicBagItem);
        _context.ProductMagicBagItems.Update(productMagicBagItemExist);
        Console.WriteLine($"Product Magic Bag Item Updated Successfully: {productMagicBagItem.Id}");
        return productMagicBagItemExist;
    }

  
}