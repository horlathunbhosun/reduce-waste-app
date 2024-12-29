using API.Models;

namespace API.Repositories;

public interface IProductRepository
{
    
    Task<Product?> GetProductById(Guid id);
    
    Task<List<Product>> GetAllProducts();
    
    Task<Product> CreateProduct(Product product);
    
    Task<Product> UpdateProduct(Product product, Guid id);
    
    Task<Product> DeleteProduct(Guid id);
}