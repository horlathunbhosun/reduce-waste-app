using API.Dtos.MagicBag;
using API.Dtos.Response;
using API.Mappers;
using API.Repositories.Interface;

namespace API.Services.MagicBag;

public class MagicBagService(IMagicBagRepository magicBagRepository, IProductMagicBagItemRepository productMagicBagItemRepository) : IMagicBagService
{
    public async Task<GenericResponse> CreateMagicBag(MagicBagRequestDto magicBagRequestDto)
        {
            try {
                var magicBagExist = await magicBagRepository.GetMagicBagByName(magicBagRequestDto.Name);
                if (magicBagExist != null)
                {
                  return  GenericResponse.FromError(new ErrorResponse("An Error occured magic bag not created", "Magic bag already exist",StatusCodes.Status400BadRequest ), StatusCodes.Status400BadRequest);
                }
                // Check if the PartnerId exists
                var partnerExists = await magicBagRepository.PartnerExists(magicBagRequestDto.PartnerId);
                if (!partnerExists)
                {
                    return GenericResponse.FromError(new ErrorResponse("An Error occured magic bag not created", "Partner does not exist", StatusCodes.Status400BadRequest), StatusCodes.Status400BadRequest);
                }

                var createMagicBag = await magicBagRepository.CreateMagicBag(magicBagRequestDto.ToMagicBagRequestDto());
                if (createMagicBag == null)
                {
                 return  GenericResponse.FromError(new ErrorResponse("An Error occured magic bag not created", "Magic bag not created",StatusCodes.Status400BadRequest ), StatusCodes.Status400BadRequest);
                }
                Console.WriteLine($"MagicBag Created Successfully: {createMagicBag}");
                return GenericResponse.FromSuccess(
                    new SuccessResponse("Magic Bag created successfully", createMagicBag.ToMagicBagResponseDto(),
                        StatusCodes.Status201Created), StatusCodes.Status201Created);
            }
            catch (Exception e)
            {
              return  GenericResponse.FromError(new ErrorResponse("An Error occured magic bag not created", $"Internal Server error {e.Message} {e.Data}",StatusCodes.Status500InternalServerError ), StatusCodes.Status500InternalServerError );
            }

        }

    public async Task<GenericResponse> CreateProductMagicBagItem(ProductMagicBagItemRequest productMagicBagItemRequest)
    {

        try
        {
            var checkProductMagicBagItem =
                await productMagicBagItemRepository.FindProductMagicItemByProductIdAndMagicBagItem((Guid)productMagicBagItemRequest!.ProductId,
                    (Guid)productMagicBagItemRequest.MagicBagId);

            if (checkProductMagicBagItem != null)
            {
                return GenericResponse.FromError(new ErrorResponse("An Error occured magic bag not created", "Product Magic Bag Item already exists", StatusCodes.Status400BadRequest), StatusCodes.Status400BadRequest);

            }

            var createProductMagicBagItem = await productMagicBagItemRepository.CreateProductMagicBagItem(productMagicBagItemRequest.ToProductMagicBagItemRequest());
            if (createProductMagicBagItem == null)
            {
                return GenericResponse.FromError(new ErrorResponse("An Error occured magic bag not created", "Magic bag not created", StatusCodes.Status400BadRequest), StatusCodes.Status400BadRequest);
            }
            Console.WriteLine($"MagicBag Created Successfully: {createProductMagicBagItem}");
            return GenericResponse.FromSuccess(
                new SuccessResponse("Magic Bag created successfully", createProductMagicBagItem.ToProductMagicBagItemResponse(),
                    StatusCodes.Status201Created), StatusCodes.Status201Created);
        }
        catch (Exception e)
        {
            return GenericResponse.FromError(new ErrorResponse("An Error occured magic bag not created", $"Internal Server error {e.Message} {e.Data}", StatusCodes.Status500InternalServerError), StatusCodes.Status500InternalServerError);
        }


    }

    public Task<GenericResponse> GetMagicBag(Guid id)
    {
        throw new NotImplementedException();
 
    }

    public async Task<GenericResponse> GetAllMagicBags()
    {
        var magicBags = await magicBagRepository.GetAllMagicBags();
        
        if (magicBags == null)
        {
            return GenericResponse.FromError(new ErrorResponse("An Error occured", "No Magic Bags",StatusCodes.Status400BadRequest ), StatusCodes.Status400BadRequest);
        }
        
        return GenericResponse.FromSuccess(
            new SuccessResponse("Magic Bag fetched successfully", magicBags.Select(magicBag => magicBag.ToMagicBagResponseDto()).ToList(),
                StatusCodes.Status200OK), StatusCodes.Status200OK);   
    }

    public async Task<GenericResponse> GetProductItems(Guid id)
    {
        var productItems = await productMagicBagItemRepository.GetProductMagicBagItems(id);
        if (productItems == null)
        {
            return GenericResponse.FromError(new ErrorResponse("An Error occured", "No Product Magic Bag Items", StatusCodes.Status400BadRequest), StatusCodes.Status400BadRequest);
        }
        
        return GenericResponse.FromSuccess(
            new SuccessResponse("Product Magic Bag Items fetched successfully", productItems.Select(productItem => productItem.ToProductMagicBagItemResponse()).ToList(),
                StatusCodes.Status200OK), StatusCodes.Status200OK);
    }

    public async Task<GenericResponse> GetAllMagicBagsByPartnerId(int partnerId)
    {
        var magicBags = await magicBagRepository.GetAllMagicBagsByPartnerId(partnerId);

        if (magicBags == null)
        {
            return GenericResponse.FromError(new ErrorResponse("An Error occured", "No Magic Bags", StatusCodes.Status400BadRequest), StatusCodes.Status400BadRequest);
        }

        return GenericResponse.FromSuccess(
            new SuccessResponse("Magic Bag fetched successfully", magicBags.Select(magicBag => magicBag.ToMagicBagResponseDto()).ToList(),
                StatusCodes.Status200OK), StatusCodes.Status200OK);
    }

    public async Task<GenericResponse> UpdateMagicBag(Guid id, MagicBagRequestDto magicBagRequestDto)
    {
        var magicBag = await magicBagRepository.GetMagicBagById(id);
        if (magicBag == null)
        {
            return GenericResponse.FromError(new ErrorResponse("An Error occured", "Magic Bag does not exist", StatusCodes.Status400BadRequest), StatusCodes.Status400BadRequest);
        }
        
        var updateMagicBag = await magicBagRepository.UpdateMagicBag(magicBagRequestDto.ToMagicBagRequestDtoUpdate(), id);
        
        if (updateMagicBag == null)
        {
            return  GenericResponse.FromError(new ErrorResponse("An Error occured magic bag not created", "Magic bag not created",StatusCodes.Status400BadRequest ), StatusCodes.Status400BadRequest);
        }
        Console.WriteLine($"MagicBag Created Successfully: {updateMagicBag}");
        return GenericResponse.FromSuccess(
            new SuccessResponse("Magic Bag updated successfully", updateMagicBag.ToMagicBagResponseDto(),
                StatusCodes.Status201Created), StatusCodes.Status201Created);
    }

    public GenericResponse DeleteMagicBag(Guid id)
    {
        var deleteMagicBag = magicBagRepository.DeleteMagicBag(id);
        if (deleteMagicBag == null)
        {
            return GenericResponse.FromError(new ErrorResponse("An Error occured", "Magic Bag does not exist", StatusCodes.Status400BadRequest), StatusCodes.Status400BadRequest);
        }
        return GenericResponse.FromSuccess(
            new SuccessResponse("Magic Bag deleted successfully", "",
                StatusCodes.Status200OK), StatusCodes.Status200OK);
    }
}