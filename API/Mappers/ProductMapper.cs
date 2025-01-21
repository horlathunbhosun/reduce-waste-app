using API.Dtos.Product;
using API.Models;

namespace API.Mappers;

public static class ProductMapper
{
    
    public static ProductResponseDto ToProductResponseDto(this Product product)
    {
        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            
            Description = product.Description,
            //MagicBagItems = product.MagicBagItems,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
    
    public static Product ToProductRequestDto(this ProductRequestDto productRequestDto)
    {
        return new Product
        {
            Name = productRequestDto.Name,
            Description = productRequestDto.Description,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        };
    }
    
}