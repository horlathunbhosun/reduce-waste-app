using API.Models;
using API.Repositories;

namespace API.Services;

public class UserServiceImpl : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserServiceImpl(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Task CreateUser(User user)
    {
        _userRepository.CreateUser(user);
        return Task.CompletedTask;
    }
}