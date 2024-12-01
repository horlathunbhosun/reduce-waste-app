using System.Security.Claims;
using API.Dtos.User;
using API.Models;

namespace API.Services.Token;

public interface ITokenService
{
    JwtToken CreateToken(Users user);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}