using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class PartnerRepository : IPartnerRepository
{
    private readonly ApplicationDbContext _context;
    
    public PartnerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Partner?> FindPartnerById(int id)
    {
        return await _context.Partners.FindAsync(id);
    }
    
    public async Task<Partner> CreatePartner(Partner partner)
    {
        await _context.Partners.AddAsync(partner);
        await _context.SaveChangesAsync();
        Console.WriteLine($"Partner Created Successfully: {partner.Id}");
        return partner;
    }
    
    public  async Task<Partner> UpdatePartner(Partner partner, string userId)
    {
        var partnerExist = await _context.Partners.FirstOrDefaultAsync(p => p.UserId == userId);
        if (partnerExist == null)
        {
            throw new InvalidOperationException("Partner does not exist");
        }
        
        _context.Entry(partnerExist).CurrentValues.SetValues(partner);
        _context.Partners.Update(partnerExist);
        Console.WriteLine($"Partner Updated Successfully: {partner.Id}");
        return partner;
    }

}