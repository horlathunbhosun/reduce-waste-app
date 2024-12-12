namespace API.Dtos.User;

public class JwtToken
{
    public string? Token { get; set; }
    
    public string? ExpiryTime { get; set; }

    public string? RefreshToken { get; set; }
    
    public UserResponseDto? UserResponseDto { get; set; }

}