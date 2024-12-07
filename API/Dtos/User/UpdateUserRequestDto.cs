using API.Enums;
using API.Models;
using System.ComponentModel.DataAnnotations;

namespace API.Dtos.User;

public class UpdateUserRequestDto
{

    public required string FullName { get; set; }
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    
    public required string Email { get; set; }
    
    public required string UserName { get; set; }

    [Phone(ErrorMessage = "Invalid Phone Number")]
    public string PhoneNumber { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "User Type is required")]
    public required string UserType { get; set; }

    
    public PartnerDto? Partner { get; set; }
}