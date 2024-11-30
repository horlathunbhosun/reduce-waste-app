using API.Dtos.User;
using API.Models;

namespace API.Services.Token;

public interface ITokenService
{
    JwtToken CreateToken(Users user);
}