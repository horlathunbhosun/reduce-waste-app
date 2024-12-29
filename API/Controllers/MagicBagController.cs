using API.Dtos.MagicBag;
using API.Services.MagicBag;
using API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


[Authorize(Roles = "Partner,Admin")]
[Route("api/magic-bag")]
[ApiController]
public class MagicBagController : ControllerBase
{
    private readonly IMagicBagService _magicBagService;
    
    public MagicBagController(IMagicBagService magicBagService)
    {
        _magicBagService = magicBagService;
    }
    
    
    [HttpPost("create")]
    public async Task<IActionResult> CreateMagicBag([FromBody] MagicBagRequestDto magicBagRequestDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            var responseEr = Constants.ErrorValidation.HanldeError("Validation failed", string.Join("; ", errors), StatusCodes.Status400BadRequest );
            return StatusCode(responseEr.StatusCode, responseEr);
        }
        var response = await _magicBagService.CreateMagicBag(magicBagRequestDto);
        return StatusCode(response.StatusCode, response);
    }
    
}

