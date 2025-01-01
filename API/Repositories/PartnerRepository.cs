using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class PartnerRepository(ApplicationDbContext context) : IPartnerRepository
{

    public async Task<Partner?> FindPartnerById(int id)
    {
        return await context.Partners.FindAsync(id);
    }

    public  Partner? FindPartnerByUserId(string id)
    {
       return context.Partners.FirstOrDefault(p => p.UserId == id);
    }

    public async Task<Partner> CreatePartner(Partner partner)
    {

        var existingPartner = await context.Partners
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Uuid == partner.Uuid);

        if (existingPartner != null)
        {
            throw new InvalidOperationException("Partner already exists.");
        }
        context.Partners.Add(partner);
        await context.SaveChangesAsync();
        Console.WriteLine($"Partner Created Successfully: {partner.Id}");
        return partner;
    }
    
    public  async Task<Partner> UpdatePartner(Partner partner, string userId)
    {
        var partnerExist = await context.Partners.FirstOrDefaultAsync(p => p.UserId == userId);
        if (partnerExist == null)
        {
            throw new InvalidOperationException("Partner does not exist");
        }

        context.Entry(partnerExist).CurrentValues.SetValues(partner);
        context.Partners.Update(partnerExist);
        Console.WriteLine($"Partner Updated Successfully: {partner.Id}");
        return partner;
    }

}