using API.Dtos.Response;
using API.Dtos.User;

namespace API.Services.UserService;

using API.Models;

public interface IUserService
{
    Task<GenericResponse> CreateUser(Users user, bool isPartner, string password);
    
    Task<Users> GetUserByVerificationCode(string verificationCode);
    
    Task<GenericResponse> VerifyUserAsync(string verificationCode);

    Task<GenericResponse> LoginUser(LoginRequestDto requestDto);
    
    Task<GenericResponse> UpdateUser(UpdateUserRequestDto user, bool isPartner);
    

    //Task<GenericResponse> RefreshToken(RefreshTokenRequest refreshTokenRequest);

}