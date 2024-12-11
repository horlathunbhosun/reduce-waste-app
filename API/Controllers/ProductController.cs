using API.Dtos.Product;
using API.Services.Product;
using API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize(Roles = "Partner,Admin")]
[Route("api/product")]
[ApiController]
public class ProductController : ControllerBase
{
    
    private readonly IProductService _productService;
    
    
    public ProductController(IProductService productService)
    {
        _productService = productService;
    }
    
    [HttpPost("create")]
    public async Task<IActionResult> CreateProduct([FromBody] ProductRequestDto productRequestDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            var responseEr = Constants.ErrorValidation.HanldeError("Validation failed", string.Join("; ", errors), StatusCodes.Status400BadRequest );
            return StatusCode(responseEr.StatusCode, responseEr);
        }
        var response = await _productService.CreateProduct(productRequestDto);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById([FromRoute] Guid id)
    {
        var response = await _productService.GetProductById(id);
        return StatusCode(response.StatusCode, response);
    }
    
    
    [HttpGet("all")]
    public async Task<IActionResult> GetAllProducts()
    {
        var response = await _productService.GetAllProducts();
        return StatusCode(response.StatusCode, response);
    }
    
    
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateProduct([FromRoute] Guid id, [FromBody] ProductRequestDto productRequestDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            var responseEr = Constants.ErrorValidation.HanldeError("Validation failed", string.Join("; ", errors), StatusCodes.Status400BadRequest );
            return StatusCode(responseEr.StatusCode, responseEr);
        }
        var response = await _productService.UpdateProduct(id, productRequestDto);
        return StatusCode(response.StatusCode, response);
    }
    
    
    [HttpDelete("{id}")]
    public  IActionResult DeleteProduct([FromRoute] Guid id)
    {
        var response = _productService.DeleteProduct(id);
        return StatusCode(response.StatusCode, response);
    }
    
}