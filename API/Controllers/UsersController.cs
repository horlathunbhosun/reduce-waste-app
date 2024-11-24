using API.Dtos.User;
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

}