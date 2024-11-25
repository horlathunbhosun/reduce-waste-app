using API.Dtos.Response;
using API.Dtos.User;
using API.Exceptions;
using API.Mappers;
using API.Services.UserService;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/user")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }
    
    
    [HttpPost("create")]
    public async Task<IActionResult> CreateUsers([FromBody] UserRequestDto userRequest)
    {
        bool isPartner = userRequest.Partner != null;
        Console.WriteLine($"Is Partner: {isPartner}");
        var user =  await _userService.CreateUser(userRequest.ToUserRequestDto(), isPartner);
        
        return Ok(new {Message= "User Created Successfully", Data = user.ToUserResponseDto()});
    }
    
    
    [HttpGet("verify/{verificationCode}")]
    public async Task<IActionResult> VerifyUser([FromRoute]string verificationCode)
    {
        try
        {
            var user = await _userService.GetUserByVerificationCode(verificationCode);
            Console.WriteLine($"User Verified Successfully: {user}");
            return Ok(new SuccessResponse("User Verified Successfully", user.ToUserResponseDto(), StatusCodes.Status200OK));

        }catch(NotFoundException e)
        {
            return NotFound(new ErrorResponse("Verification code is invalid", e.Message, StatusCodes.Status404NotFound));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            
            return StatusCode(500, new ErrorResponse("An unexpected error occurred", e.Message, StatusCodes.Status500InternalServerError));

        }
    }

}