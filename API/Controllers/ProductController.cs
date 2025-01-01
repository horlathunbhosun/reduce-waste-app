using API.Dtos.Product;
using API.Services.Product;
using API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize(Roles = "Partner,Admin")]
[Route("api/product")]
[ApiController]
public class ProductController(IProductService productService) : ControllerBase
{
    

    [HttpPost("create")]
    public async Task<IActionResult> CreateProduct([FromBody] ProductRequestDto productRequestDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            var responseEr = Constants.ErrorValidation.HanldeError("Validation failed", string.Join("; ", errors), StatusCodes.Status400BadRequest );
            return StatusCode(responseEr.StatusCode, responseEr);
        }
        var response = await productService.CreateProduct(productRequestDto);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById([FromRoute] Guid id)
    {
        var response = await productService.GetProductById(id);
        return StatusCode(response.StatusCode, response);
    }
    
    
    [HttpGet("all")]
    public async Task<IActionResult> GetAllProducts()
    {
        var response = await productService.GetAllProducts();
        return StatusCode(response.StatusCode, response);
    }
    
    
    [HttpPatch("{id}/update")]
    public async Task<IActionResult> UpdateProduct([FromRoute] Guid id, [FromBody] ProductRequestDto productRequestDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            var responseEr = Constants.ErrorValidation.HanldeError("Validation failed", string.Join("; ", errors), StatusCodes.Status400BadRequest );
            return StatusCode(responseEr.StatusCode, responseEr);
        }
        var response = await productService.UpdateProduct(id, productRequestDto);
        return StatusCode(response.StatusCode, response);
    }
    
    
    [HttpDelete("{id}/delete")]
    public  IActionResult DeleteProduct([FromRoute] Guid id)
    {
        var response = productService.DeleteProduct(id);
        return StatusCode(response.StatusCode, response);
    }
    
}