using API.Dtos.MagicBag;
using API.Services.MagicBag;
using API.Services.UserService;
using API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


[Authorize(Roles = "Partner,Admin")]
[Route("api/magic-bag")]
[ApiController]
public class MagicBagController(IMagicBagService magicBagService, IUserService userService) : ControllerBase
{

    [HttpPost("create")]
    public async Task<IActionResult> CreateMagicBag([FromBody] MagicBagRequestDto magicBagRequestDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            var responseEr = Constants.ErrorValidation.HanldeError("Validation failed", string.Join("; ", errors), StatusCodes.Status400BadRequest );
            return StatusCode(responseEr.StatusCode, responseEr);
        }
        var response = await magicBagService.CreateMagicBag(magicBagRequestDto);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPatch("{id}/update")]
    public async Task<IActionResult> CreateMagicBag([FromRoute] Guid id, [FromBody] MagicBagRequestDto magicBagRequestDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            var responseEr = Constants.ErrorValidation.HanldeError("Validation failed", string.Join("; ", errors), StatusCodes.Status400BadRequest );
            return StatusCode(responseEr.StatusCode, responseEr);
        }
        var response = await magicBagService.UpdateMagicBag(id, magicBagRequestDto);
        return StatusCode(response.StatusCode, response);
    }
    
    
    [HttpGet("get-all")]
    public async Task<IActionResult> GetAllMagicBags()
    {
        var response = await magicBagService.GetAllMagicBags();
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpDelete("{id}/delete")]
    public  IActionResult DeleteProduct([FromRoute] Guid id)
    {
        var response = magicBagService.DeleteMagicBag(id);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("get-partners-magicbags")]
    public async Task<IActionResult> GetPartnersMagicBags()
    {
        var partner = userService.GetPartnerByUserId(Constants.GetUserId(HttpContext));

        var response = await magicBagService.GetAllMagicBagsByPartnerId(partner.Id);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpGet("get-magicbag/{id}")]
    public async Task<IActionResult> GetMagicBag(Guid id)
    {
        var response = await magicBagService.GetMagicBag(id);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("add-magic-bag-item")]
    public async Task<IActionResult> AddMagicBagItem([FromBody] ProductMagicBagItemRequest productMagicBagItemRequest)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            var responseEr = Constants.ErrorValidation.HanldeError("Validation failed", string.Join("; ", errors), StatusCodes.Status400BadRequest);
            return StatusCode(responseEr.StatusCode, responseEr);
        }
        var response = await magicBagService.CreateProductMagicBagItem(productMagicBagItemRequest);
        return StatusCode(response.StatusCode, response);
    }





    //public async Task<IActionResult> GetMagicBag(Guid id)
    //{
    //    var response = await _magicBagService.GetMagicBag(id);
    //    return StatusCode(response.StatusCode, response);
    //}

}

