using API.Dtos.User;
using API.Mappers;
using API.Services;
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
        await _userService.CreateUser(userRequest.ToUserRequestDto());

        return Ok("User Created Successfully");
    }

}