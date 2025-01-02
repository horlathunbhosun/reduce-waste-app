using API.Data;
using API.Models;
using API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class ProductMagicBagItemRepository(ApplicationDbContext context) : IProductMagicBagItemRepository
{

    public async Task<ProductMagicBagItem?> GetProductMagicBagItemById(Guid id)
    {
        return await context.ProductMagicBagItems.FindAsync(id);   
    }

  

    public async Task<ProductMagicBagItem> CreateProductMagicBagItem(ProductMagicBagItem productMagicBagItem)
    {
        await context.ProductMagicBagItems.AddAsync(productMagicBagItem);
        await context.SaveChangesAsync();
        Console.WriteLine($"Product Magic Bag Item Created Successfully: {productMagicBagItem.Id}");
        return productMagicBagItem;
    }

    public async Task<ProductMagicBagItem> UpdateProductMagicBagItem(ProductMagicBagItem productMagicBagItem)
    {
        var productMagicBagItemExist = await context.ProductMagicBagItems.FindAsync(productMagicBagItem.Id);
        if (productMagicBagItemExist == null)
        {
            throw new InvalidOperationException("Product Magic Bag Item does not exist");
        }

        context.Entry(productMagicBagItemExist).CurrentValues.SetValues(productMagicBagItem);
        context.ProductMagicBagItems.Update(productMagicBagItemExist);
        Console.WriteLine($"Product Magic Bag Item Updated Successfully: {productMagicBagItem.Id}");
        return productMagicBagItemExist;
    }


    public async Task<ProductMagicBagItem?> FindProductMagicItemByProductIdAndMagicBagItem(Guid productId, Guid magicBagId)
    {
        var productMagicBagItem = await context.ProductMagicBagItems.FirstOrDefaultAsync(p => p.ProductId == productId && p.MagicBagId == magicBagId);

        return productMagicBagItem;
    }


}