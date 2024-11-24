namespace API.Services.UserService;

using API.Models;

public interface IUserService
{
    Task<Users> CreateUser(Users user, bool isPartner);

}