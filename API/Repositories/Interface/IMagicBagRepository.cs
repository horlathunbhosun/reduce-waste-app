using API.Models;

namespace API.Repositories.Interface;

public interface IMagicBagRepository
{
    Task<MagicBag?> GetMagicBagById(Guid id);
    Task<MagicBag?> GetMagicBagByIdNew(Guid id);

    Task<MagicBag?> GetMagicBagByName(string? name);
    
    
    Task<List<MagicBag>> GetAllMagicBags();
    
    Task<List<MagicBag>> GetAllMagicBagsByPartnerId(int partnerId);
    
    Task<MagicBag> CreateMagicBag(MagicBag magicBag);
    
    
    Task<bool> PartnerExists(int partnerId);


    Task<MagicBag> UpdateMagicBag(MagicBag magicBag, Guid id);
    
    Task<MagicBag> DeleteMagicBag(Guid id);
}