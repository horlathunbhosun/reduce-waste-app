using System.IdentityModel.Tokens.Jwt;
using API.Dtos.User;
using API.Services.UserService;
using API.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers;


[Authorize]
[Route("api/user")]
[ApiController]
public class UserController(IUserService userService) : ControllerBase
{
    
    
    [HttpGet("profile")]
    public async Task<IActionResult> Profile()
    {
        var userDetail = await userService.UserProfile(GetEmail());        
        return StatusCode(StatusCodes.Status200OK, userDetail);
    }
    
    
    [HttpPatch("profile/update")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserRequestDto user)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            var responseEr = Constants.ErrorValidation.HanldeError("Validation failed", string.Join("; ", errors), StatusCodes.Status400BadRequest );
            return StatusCode(responseEr.StatusCode, responseEr);
        }
        var isPartner = CheckRole();
        var email = GetEmail();
        var response = await userService.UpdateUser(user,isPartner,email);
        return StatusCode(response.StatusCode, response);
    }
    
    
    private string GetEmail()
    {
        var currentUser = HttpContext.User;
        var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == Constants.EmailClamValue.Value)?.Value;
        if (email == null)
        {
            return string.Empty;
        }

        return email;
    }
    
    private bool CheckRole()
    {
        var currentUser = HttpContext.User;
        var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userRole")?.Value;
        return role == "Partner";
    }
}