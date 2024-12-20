using API.Models;

namespace API.Repositories;

public interface IPartnerRepository
{
    Task<Partner?> FindPartnerById(int id);
    
    Task<Partner> CreatePartner(Partner partner);
    
    Task<Partner> UpdatePartner(Partner partner, string userId);
}