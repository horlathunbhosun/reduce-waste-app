using System.ComponentModel.DataAnnotations;
using API.Enums;

namespace API.Models;

public class User
{
    [Key]
    public int Id { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }

    public required string Password { get; set; }

    public string PhoneNumber { get; set; } = string.Empty;

    public string? VerificationCode { get; set; }

    public UserType UserType { get; set; }

    public bool IsVerified { get; set; }

    public Status Status { get; set; }

    public Partner? Partner { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

}