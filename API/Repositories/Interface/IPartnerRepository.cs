using API.Models;

namespace API.Repositories;

public interface IPartnerRepository
{
    Task<Partner?> FindPartnerById(int id);
    
    Partner? FindPartnerByUserId(String id);
    
    Task<Partner> CreatePartner(Partner partner);
    
    Task<Partner> UpdatePartner(Partner partner, string userId);
}