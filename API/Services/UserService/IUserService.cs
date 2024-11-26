using API.Dtos.Response;

namespace API.Services.UserService;

using API.Models;

public interface IUserService
{
    Task<GenericResponse> CreateUser(Users user, bool isPartner);
    
    Task<Users> GetUserByVerificationCode(string verificationCode);
    
    Task<GenericResponse> VerifyUserAsync(string verificationCode);

}