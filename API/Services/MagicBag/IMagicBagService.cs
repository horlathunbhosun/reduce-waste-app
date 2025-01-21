using API.Dtos.MagicBag;
using API.Dtos.Response;

namespace API.Services.MagicBag;

public interface IMagicBagService 
{
    Task<GenericResponse> CreateMagicBag(MagicBagRequestDto magicBagRequestDto);
    
    Task<GenericResponse> CreateProductMagicBagItem(ProductMagicBagItemRequest productMagicBagItemRequest);

    Task<GenericResponse> GetMagicBag(Guid id);
    
    Task<GenericResponse> GetAllMagicBags();
    
    Task<GenericResponse> GetProductItems(Guid id);
    
    Task<GenericResponse> GetAllMagicBagsByPartnerId(int partnerId);

    Task<GenericResponse> UpdateMagicBag(Guid id,MagicBagRequestDto magicBagRequestDto);
    
    GenericResponse DeleteMagicBag(Guid id);
    
    
}