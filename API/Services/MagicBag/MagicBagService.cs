using API.Dtos.MagicBag;
using API.Dtos.Response;
using API.Mappers;
using API.Repositories.Interface;

namespace API.Services.MagicBag;

public class MagicBagService : IMagicBagService
{
    
    private readonly IMagicBagRepository _magicBagRepository;
    
    private readonly IProductMagicBagItemRepository _productMagicBagItemRepository;
    
    public MagicBagService(IMagicBagRepository magicBagRepository, IProductMagicBagItemRepository productMagicBagItemRepository)
    {
        _magicBagRepository = magicBagRepository;
        _productMagicBagItemRepository = productMagicBagItemRepository;
    }
    
        public async Task<GenericResponse> CreateMagicBag(MagicBagRequestDto magicBagRequestDto)
        {
            try {
                var magicBagExist = await _magicBagRepository.GetMagicBagByName(magicBagRequestDto.Name);
                if (magicBagExist != null)
                {
                  return  GenericResponse.FromError(new ErrorResponse("An Error occured magic bag not created", "Magic bag already exist",StatusCodes.Status400BadRequest ), StatusCodes.Status400BadRequest);
                }
                
                var createMagicBag = await _magicBagRepository.CreateMagicBag(magicBagRequestDto.ToMagicBagRequestDto());
                if (createMagicBag == null)
                {
                 return  GenericResponse.FromError(new ErrorResponse("An Error occured magic bag not created", "Magic bag not created",StatusCodes.Status400BadRequest ), StatusCodes.Status400BadRequest);
                }
                Console.WriteLine($"MagicBag Created Successfully: {createMagicBag}");
                return GenericResponse.FromSuccess(
                    new SuccessResponse("Magic Bag created successfully", createMagicBag?.ToMagicBagResponseDto(),
                        StatusCodes.Status201Created), StatusCodes.Status201Created);
            }
            catch (Exception e)
            {
              return  GenericResponse.FromError(new ErrorResponse("An Error occured magic bag not created", $"Internal Server error {e.Message} {e.Data}",StatusCodes.Status500InternalServerError ), StatusCodes.Status500InternalServerError );
            }

        }

    public Task<GenericResponse> GetMagicBag(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<GenericResponse> GetAllMagicBags()
    {
        throw new NotImplementedException();
    }

    public Task<GenericResponse> UpdateMagicBag(MagicBagRequestDto magicBagRequestDto)
    {
        throw new NotImplementedException();
    }

    public Task<GenericResponse> DeleteMagicBag(Guid id)
    {
        throw new NotImplementedException();
    }
}