using API.Dtos.MagicBag;
using API.Dtos.Response;

namespace API.Services.MagicBag;

public interface IMagicBagService 
{
    Task<GenericResponse> CreateMagicBag(MagicBagRequestDto magicBagRequestDto);

    Task<GenericResponse> GetMagicBag(Guid id);
    
    Task<GenericResponse> GetAllMagicBags();
    
    Task<GenericResponse> UpdateMagicBag(MagicBagRequestDto magicBagRequestDto);
    
    Task<GenericResponse> DeleteMagicBag(Guid id);
    
    
}