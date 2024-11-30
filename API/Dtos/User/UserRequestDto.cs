using API.Enums;
using API.Models;
using System.ComponentModel.DataAnnotations;

namespace API.Dtos.User;

public class UserRequestDto
{
    [Required(ErrorMessage = "Full Name is required")]

    public required string FullName { get; set; }
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public required string Email { get; set; }
    public required string UserName { get; set; }

    [Phone(ErrorMessage = "Invalid Phone Number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*\W).+$", ErrorMessage = "Passwords must have at least one non alphanumeric character, one digit ('0'-'9'), and one uppercase ('A'-'Z') character.")]
    public required string Password { get; set; }
    
    
    [Required(ErrorMessage = "User Type is required")]
    public required string UserType { get; set; }

    
    public PartnerDto? Partner { get; set; }

}