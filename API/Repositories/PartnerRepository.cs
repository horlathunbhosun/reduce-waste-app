using API.Data;
using API.Models;

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
        return await _context.Partner.FindAsync(id);
    }
    
    public async Task<Partner> CreatePartner(Partner partner)
    {
        await _context.Partner.AddAsync(partner);
        await _context.SaveChangesAsync();
        Console.WriteLine($"Partner Created Successfully: {partner.Id}");
        return partner;
    }

}