using API.Models;

namespace API.Repositories.Interface;

public interface IMagicBagRepository
{
    Task<MagicBag?> GetMagicBagById(Guid id);
    
    Task<MagicBag?> GetMagicBagByName(string? name);
    
    Task<List<MagicBag>> GetAllMagicBags();
    
    Task<List<MagicBag>> GetAllMagicBagsByPartnerId(int partnerId);
    
    Task<MagicBag> CreateMagicBag(MagicBag magicBag);
    
    Task<MagicBag> UpdateMagicBag(MagicBag magicBag);
    
    Task<MagicBag> DeleteMagicBag(Guid id);
}