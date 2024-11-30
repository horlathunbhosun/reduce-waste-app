using System.ComponentModel.DataAnnotations;

namespace API.Dtos.User;

public class LoginRequestDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*\W).+$", ErrorMessage = "Passwords must have at least one non alphanumeric character, one digit ('0'-'9'), and one uppercase ('A'-'Z') character.")]
    public string Password { get; set; }
}