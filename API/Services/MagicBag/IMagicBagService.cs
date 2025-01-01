using API.Dtos.MagicBag;
using API.Dtos.Response;

namespace API.Services.MagicBag;

public interface IMagicBagService 
{
    Task<GenericResponse> CreateMagicBag(MagicBagRequestDto magicBagRequestDto);

    Task<GenericResponse> GetMagicBag(Guid id);
    
    Task<GenericResponse> GetAllMagicBags();
    
    Task<GenericResponse> GetAllMagicBagsByPartnerId(int partnerId);

    Task<GenericResponse> UpdateMagicBag(Guid id,MagicBagRequestDto magicBagRequestDto);
    
    Task<GenericResponse> DeleteMagicBag(Guid id);
    
    
}