using API.Enums;
using API.Models;

namespace API.Dtos.User;

public class UserRequestDto
{
    public required string FullName { get; set; }
    public required string Email { get; set; }

    public string PhoneNumber { get; set; } = string.Empty;

    public required string Password { get; set; }
    public UserType UserType { get; set; }

    
    public Partner? Partner { get; set; }

}