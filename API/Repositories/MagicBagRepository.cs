using API.Data;
using API.Models;
using API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class MagicBagRepository : IMagicBagRepository
{
    private readonly ApplicationDbContext _context;
    
    public MagicBagRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<MagicBag?> GetMagicBagById(Guid id)
    {
        return await _context.MagicBags.Include("MagicBagItems").Include("Partners").FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<MagicBag?> GetMagicBagByName(string? name)
    {
        return await _context.MagicBags.Include("MagicBagItems").Include("Partner").FirstOrDefaultAsync(m => m.Name == name);
    }

    public async Task<List<MagicBag>> GetAllMagicBags()
    {
        return await _context.MagicBags
                .Include(m => m.MagicBagItems)
                 .Include(m => m.Partner)
                    .ThenInclude(p => p.User).ToListAsync();
    }

    public async Task<List<MagicBag>> GetAllMagicBagsByPartnerId(int partnerId)
    {
        return await _context.MagicBags
            .Include(m => m.MagicBagItems)
              .Include(m => m.Partner)
                .ThenInclude(p => p.User)
                    .Where(m => m.PartnerId == partnerId).ToListAsync();
    }

    public async Task<MagicBag> CreateMagicBag(MagicBag magicBag)
    {
        await _context.MagicBags.AddAsync(magicBag);
        await _context.SaveChangesAsync();
        Console.WriteLine($"MagicBag Created Successfully: {magicBag.Id}");
        return magicBag;
    }

    public async Task<MagicBag> UpdateMagicBag(MagicBag magicBag)
    {
        var magicBagExist = await _context.MagicBags.FindAsync(magicBag.Id);
        if (magicBagExist == null)
        {
            throw new InvalidOperationException("MagicBag does not exist");
        }
        
        _context.Entry(magicBagExist).CurrentValues.SetValues(magicBag);
        _context.MagicBags.Update(magicBagExist);
        Console.WriteLine($"MagicBag Updated Successfully: {magicBag.Id}");
        return magicBagExist;
    }

    public Task<MagicBag> DeleteMagicBag(Guid id)
    {
        var magicBag = _context.MagicBags.Find(id);
        if (magicBag == null)
        {
            throw new InvalidOperationException("MagicBag does not exist");
        }
        var magicBagItems = _context.ProductMagicBagItems.Where(mbi => mbi.MagicBagId == id).ToList();
        _context.ProductMagicBagItems.RemoveRange(magicBagItems);
        _context.MagicBags.Remove(magicBag);
        _context.SaveChanges();
        
        return Task.FromResult(magicBag);
    }
}