namespace API.Services;

using API.Models;

public interface IUserService
{
    Task CreateUser(User user);

}