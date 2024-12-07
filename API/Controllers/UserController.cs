using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


[Authorize]
[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    
    [HttpGet("profile")]
    public IActionResult Profile()
    {
        var currentUser = HttpContext.User;
        
        var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
        Console.WriteLine($"Email: {email}");
        currentUser.Claims.ToList().ForEach(claim => {
            System.Console.WriteLine($"Claim Type: {claim.Type} - Claim Value: {claim.Value}");
        });
        return StatusCode(StatusCodes.Status200OK, currentUser.Claims.ToList());
      //  return  StatusCode(StatusCodes.Status200OK, currentUser);
        // var user = await _userService.UserById(User.FindFirstValue(ClaimTypes.NameIdentifier));
        // return StatusCode(StatusCodes.Status200OK, user);
    }
}