namespace API.Services;

using API.Models;

public interface IUserService
{
    Task<Users> CreateUser(Users user, bool isPartner);

}