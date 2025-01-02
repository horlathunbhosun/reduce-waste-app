using API.Dtos.MagicBag;
using API.Models;

namespace API.Mappers;

public static class MagicBagMapper
{
    public static MagicBagResponseDto ToMagicBagResponseDto(this MagicBag magicBag)
    {
            return new MagicBagResponseDto
            {
                Id = magicBag.Id,
                Name = magicBag.Name,
                Description = magicBag.Description,
                BagPrice = magicBag.BagPrice,
                Status = magicBag.Status,
                Partner = magicBag?.Partner?.ToPartnerResponseDto(),
                MagicBagItems = magicBag?.MagicBagItems?.Select(x => x.ToProductMagicBagItemResponse()).ToList(),
                CreatedAt = magicBag!.CreatedAt,
                UpdatedAt = magicBag.UpdatedAt
            };
    }
    
    public static MagicBag ToMagicBagRequestDto(this MagicBagRequestDto magicBagRequestDto)
    {
        return new MagicBag()
        {
            Name = magicBagRequestDto.Name,
            Description = magicBagRequestDto.Description,
            BagPrice = magicBagRequestDto.BagPrice,
            PartnerId = magicBagRequestDto.PartnerId,
           // MagicBagItems = magicBagRequestDto.MagicBagItems?.Select(x => x.ToProductMagicBagItemRequest()).ToList(),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        };
    }
    
    
      
    public static ProductMagicBagItem ToProductMagicBagItemRequest(this ProductMagicBagItemRequest productMagicBagItemRequestDto)
    {
        
        return new ProductMagicBagItem
        {
            MagicBagId = productMagicBagItemRequestDto.MagicBagId,
            ProductId = productMagicBagItemRequestDto.ProductId,
            Quantity = productMagicBagItemRequestDto.Quantity,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        };
    }
    
    public static ProductMagicBagItemResponse ToProductMagicBagItemResponse(this ProductMagicBagItem productMagicBagItem)
    {
        return new ProductMagicBagItemResponse
        {
            Id = productMagicBagItem.Id,
            Products = productMagicBagItem.Products?.ToProductResponseDto(),
            MagicBagId = (Guid)productMagicBagItem.MagicBagId,
           //MagicBag = productMagicBagItem.MagicBag?.ToMagicBagResponseDto(),
            Quantity = productMagicBagItem.Quantity,
            CreatedAt = productMagicBagItem.CreatedAt,
            UpdatedAt = productMagicBagItem.UpdatedAt
        };
    }
  
}