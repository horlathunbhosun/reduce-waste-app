using API.Data;
using API.Models;
using API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class MagicBagRepository(ApplicationDbContext context) : IMagicBagRepository
{

    public async Task<MagicBag?> GetMagicBagById(Guid id)
    {
        return await context.MagicBags.Include("MagicBagItems").Include("Partners").FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<MagicBag?> GetMagicBagByName(string? name)
    {
        return await context.MagicBags.Include("MagicBagItems").Include("Partner").FirstOrDefaultAsync(m => m.Name == name);
    }

    public async Task<List<MagicBag>> GetAllMagicBags()
    {
        return await context.MagicBags
                .Include(m => m.MagicBagItems)
                 .Include(m => m.Partner)
                    .ThenInclude(p => p.User).ToListAsync();
    }

    public async Task<List<MagicBag>> GetAllMagicBagsByPartnerId(int partnerId)
    {
        return await context.MagicBags
            .Include(m => m.MagicBagItems)
                    .Where(m => m.PartnerId == partnerId).ToListAsync();
    }

    public async Task<MagicBag> CreateMagicBag(MagicBag magicBag)
    {
        await context.MagicBags.AddAsync(magicBag);
        await context.SaveChangesAsync();
        Console.WriteLine($"MagicBag Created Successfully: {magicBag.Id}");
        return magicBag;
    }

    public async Task<ProductMagicBagItem> CreateProductMagicBagItem(ProductMagicBagItem productMagicBagItem)
    {
      await context.ProductMagicBagItems.AddAsync(productMagicBagItem);
      await context.SaveChangesAsync();
      Console.WriteLine($"ProductMagicBagItem Created Successfully: {productMagicBagItem.Id}");
      return productMagicBagItem;
    }

    public async Task<ProductMagicBagItem?> FindProductMagicItemByProductIdAndMagicBagItem(Guid productId, Guid magicBagId)
    {
       var productMagicBagItem = await context.ProductMagicBagItems.FirstOrDefaultAsync(p => p.ProductId == productId && p.MagicBagId == magicBagId);
       
       return productMagicBagItem;
    }

    public async Task<MagicBag> UpdateMagicBag(MagicBag magicBag)
    {
        var magicBagExist = await context.MagicBags.FindAsync(magicBag.Id);
        if (magicBagExist == null)
        {
            throw new InvalidOperationException("MagicBag does not exist");
        }

        context.Entry(magicBagExist).CurrentValues.SetValues(magicBag);
        context.MagicBags.Update(magicBagExist);
        Console.WriteLine($"MagicBag Updated Successfully: {magicBag.Id}");
        return magicBagExist;
    }

    public Task<MagicBag> DeleteMagicBag(Guid id)
    {
        var magicBag = context.MagicBags.Find(id);
        if (magicBag == null)
        {
            throw new InvalidOperationException("MagicBag does not exist");
        }
        var magicBagItems = context.ProductMagicBagItems.Where(mbi => mbi.MagicBagId == id).ToList();
        context.ProductMagicBagItems.RemoveRange(magicBagItems);
        context.MagicBags.Remove(magicBag);
        context.SaveChanges();
        
        return Task.FromResult(magicBag);
    }
}