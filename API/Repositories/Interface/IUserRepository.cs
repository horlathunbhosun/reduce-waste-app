using API.Dtos.User;
using API.Models;

namespace API.Repositories;

public interface IUserRepository
{
    Task<Users?> UserById(string id);

    Task<Users?> FindUserByEmail(string email);

    Task<Users?> FindUserByPhoneNumber(string phoneNumber);

    Task<Users?> FindUserByVerificationCode(string verificationCode);

    Task<Users> CreateUser(Users user,string password);
    
    //Task<Users> UpdateUser(UpdateUserRequestDto user, string userId);
    
    Task<Users> UpdateUser(Users user);

    
}