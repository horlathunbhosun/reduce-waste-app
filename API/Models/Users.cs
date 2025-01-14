using System.ComponentModel.DataAnnotations;
using API.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Models;

public class Users : IdentityUser
{
    
    public required string FullName { get; set; }

    public string? VerificationCode { get; set; }

    [Required]
    public string? UserType { get; set; }
    
    public bool IsVerified { get; set; }

    public Status Status { get; set; } = Status.Pending;

    public virtual Partner? Partner { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public  List<Transactions>? UserTransactions { get; set; }

}