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
    
    Task<GenericResponse> UpdateUser(UpdateUserRequestDto user, bool isPartner, string email);
    
    Task<GenericResponse> UserProfile(string email);
    
    Partner GetPartnerByUserId(string userId);
    
    Task<GenericResponse> GetAllUsers();


}