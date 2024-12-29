using API.Data;
using API.Exceptions;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class ProductRepository : IProductRepository
{
    
    
    private readonly ApplicationDbContext _context;
    
    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    
    public async Task<Product?> GetProductById(Guid id)
    {
       var product = await  _context.Products.FindAsync(id);
       
       return product;
    }

    public Task<List<Product>> GetAllProducts()
    {
        var products = _context.Products.ToListAsync();
        
        return products;
    }

    public async Task<Product> CreateProduct(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        Console.WriteLine($"Product Created Successfully: {product.Id}");
        return product;
    }

    public  async Task<Product> UpdateProduct(Product product, Guid id)
    {
        var productExist = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (productExist == null)
        {
            throw new NotFoundException("Product does not exist");
        }

        
        _context.Entry(productExist).CurrentValues.SetValues(product);
        var data =   _context.Products.Update(productExist);
        Console.WriteLine($"Product Updated Successfully: {product.Id}");
        Console.WriteLine($"Product Updated ENtity Successfully: {data.Entity.Name}" );
        await _context.SaveChangesAsync();
        return productExist;
    }

    public  Task<Product> DeleteProduct(Guid id)
    {
        var product =  _context.Products.Find(id);
        if (product == null)
        {
            throw new NotFoundException("Product not found");
        }
        
        _context.Products.Remove(product);
        _context.SaveChangesAsync();
        
        return Task.FromResult(product);
    }
}