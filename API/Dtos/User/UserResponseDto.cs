using API.Enums;

namespace API.Dtos.User;
using API.Models;

public class UserResponseDto
{
    public Guid Id { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }


    public string PhoneNumber { get; set; } = string.Empty;

    public UserType UserType { get; set; }

    public bool IsVerified { get; set; }

    public Status Status { get; set; }

    public PartnerResponseDto? Partner { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}