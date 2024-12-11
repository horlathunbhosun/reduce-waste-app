using API.Dtos.Product;
using API.Dtos.Response;

namespace API.Services.Product;

public interface IProductService
{
    Task<GenericResponse> CreateProduct(ProductRequestDto productRequestDto);
    
    Task<GenericResponse> GetProductById(Guid id);
    
    Task<GenericResponse> GetAllProducts();
    
    Task<GenericResponse> UpdateProduct(Guid id, ProductRequestDto productRequestDto);
    
    GenericResponse DeleteProduct(Guid id);
    
    
}