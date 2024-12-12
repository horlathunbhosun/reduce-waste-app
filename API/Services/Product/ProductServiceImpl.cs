using API.Dtos.Product;
using API.Dtos.Response;
using API.Mappers;
using API.Repositories;

namespace API.Services.Product;

public class ProductServiceImpl : IProductService
{

    private readonly IProductRepository _productRepository;
    
    
    public ProductServiceImpl(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    
    
    public async Task<GenericResponse> CreateProduct(ProductRequestDto productRequestDto)
    {
        try
        {
            var createdProduct = await _productRepository.CreateProduct(productRequestDto.ToProductRequestDto());
            if (createdProduct == null)
            {
                return GenericResponse.FromError(new ErrorResponse("An Error occured product not created", "Product not created",StatusCodes.Status400BadRequest ), StatusCodes.Status400BadRequest);
            }
        
            return GenericResponse.FromSuccess(new SuccessResponse("Product created successfully", createdProduct.ToProductResponseDto(), StatusCodes.Status201Created), StatusCodes.Status201Created);
        }catch (Exception e)
        {
            Console.WriteLine(e);
            return GenericResponse.FromError(new ErrorResponse("An Error occured product not created", "Internal Server error",StatusCodes.Status500InternalServerError ), StatusCodes.Status500InternalServerError );
        }

    }

    public async Task<GenericResponse> GetProductById(Guid id)
    {
        var product = await _productRepository.GetProductById(id);
        if (product == null)
        {
            return GenericResponse.FromError(new ErrorResponse("An Error occured product not found", "Product not found",StatusCodes.Status404NotFound ), StatusCodes.Status404NotFound);
        }
        
        return GenericResponse.FromSuccess(new SuccessResponse("Product found successfully", product.ToProductResponseDto(), StatusCodes.Status200OK), StatusCodes.Status200OK);
    }

    public async Task<GenericResponse> GetAllProducts()
    {
       var products = await _productRepository.GetAllProducts();
         if (products == null)
         {
              return GenericResponse.FromError(new ErrorResponse("An Error occured product not found", "Product not found",StatusCodes.Status404NotFound ), StatusCodes.Status404NotFound);
         }
         return GenericResponse.FromSuccess(new SuccessResponse("Product found successfully", products.Select(product => product.ToProductResponseDto()).ToList(), StatusCodes.Status200OK), StatusCodes.Status200OK);
    }

    public async Task<GenericResponse> UpdateProduct(Guid id, ProductRequestDto productRequestDto)
    {
        try
        {
            var product = await _productRepository.GetProductById(id);
            if (product == null)
            {
                return GenericResponse.FromError(new ErrorResponse("An Error occured product not found", "Product not found",StatusCodes.Status404NotFound ), StatusCodes.Status404NotFound);
            } 
            var updatedProduct = await _productRepository.UpdateProduct(productRequestDto.ToProductRequestDto());
            if (updatedProduct == null)
            {
                return GenericResponse.FromError(new ErrorResponse("An Error occured product not updated", "Product not updated",StatusCodes.Status400BadRequest ), StatusCodes.Status400BadRequest);
            }
            return GenericResponse.FromSuccess(new SuccessResponse("Product updated successfully", updatedProduct.ToProductResponseDto(), StatusCodes.Status200OK), StatusCodes.Status200OK);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return GenericResponse.FromError(new ErrorResponse("An Error occured product not updated", "Internal Server error",StatusCodes.Status500InternalServerError ), StatusCodes.Status500InternalServerError );
        }

    }

    public  GenericResponse DeleteProduct(Guid id)
    {
       var product =  _productRepository.DeleteProduct(id);
         if (product == null)
         {
              return GenericResponse.FromError(new ErrorResponse("An Error occured product not found", "Product not found",StatusCodes.Status404NotFound ), StatusCodes.Status404NotFound);
         }
         return GenericResponse.FromSuccess(new SuccessResponse("Product found successfully", "", StatusCodes.Status204NoContent), StatusCodes.Status204NoContent);
    }
}