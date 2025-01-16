
namespace API.Dtos.User;

public class PartnerResponseDto
{
    
    public Guid Id { get; set; }
    
    public int PartnerId { get; set; }

    //Navigation Property
    public UserResponseDto?  User { get; set; }

    public int BusinessNumber { get; set; }

    public string Logo { get; set; } = string.Empty;

    public string Address   { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}