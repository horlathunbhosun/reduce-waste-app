using System.ComponentModel.DataAnnotations;
using API.Enums;
using Microsoft.EntityFrameworkCore;

namespace API.Models;

public class Users
{
    [Key]
    public int Id { get; set; }
    
    public Guid Uuid { get; set; } = System.Guid.NewGuid();

    public required string FullName { get; set; }
    
    [Required]
    [EmailAddress] 
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }
    
    public string PhoneNumber { get; set; } = string.Empty;

    public string? VerificationCode { get; set; }

    [Required]
    public UserType UserType { get; set; }
    
    public bool IsVerified { get; set; }

    public Status Status { get; set; } = Status.Pending;

    public Partner? Partner { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

}