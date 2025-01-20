using API.Dtos.Response;
using API.Dtos.User;
using API.Exceptions;
using API.Mappers;
using API.Services.UserService;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


[Route("api/user")]
[ApiController]
public class RegistrationController(IUserService userService) : ControllerBase
{
   
    
    
    [HttpPost("create")]
    public async Task<IActionResult> CreateUsers([FromBody] UserRequestDto userRequest)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            var responseEr = HanldeError("Validation failed", string.Join("; ", errors), StatusCodes.Status400BadRequest );
            return StatusCode(responseEr.StatusCode, responseEr);
        }
        bool isPartner = userRequest.Partner != null;
        var response =  await userService.CreateUser(userRequest.ToUserRequestDto(), isPartner, userRequest.Password);
        return StatusCode(response.StatusCode, response);
    }
    
    
    [HttpGet("verify/{verificationCode}")]
    public async Task<IActionResult> VerifyUser([FromRoute] string verificationCode)
    {
        if (string.IsNullOrEmpty(verificationCode))
        {
            var responseEr = HanldeError("Validation failed", "Verification code is required", StatusCodes.Status400BadRequest);
            return StatusCode(responseEr.StatusCode, responseEr);
        }
        var response = await userService.VerifyUserAsync(verificationCode);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
    {
        
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            var responseEr = HanldeError("Validation failed", string.Join("; ", errors), StatusCodes.Status400BadRequest );
            return StatusCode(responseEr.StatusCode, responseEr);
        }

        var response = await userService.LoginUser(loginRequestDto);
        return StatusCode(response.StatusCode, response);
    }


    private GenericResponse HanldeError(string message, string error, int statusCode)
    {
     return   GenericResponse.FromError(
            new ErrorResponse(message, error,
                statusCode), statusCode);
    }
    
    

}