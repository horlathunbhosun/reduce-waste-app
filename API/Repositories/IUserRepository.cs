using API.Models;

namespace API.Repositories;

public interface IUserRepository
{
    Task<User?> UserById(int id);

    Task<User?> UserByEmail(string email);

    Task<User?> FindUserByPhoneNumber(string phoneNumber);

    Task<User?> FindUserByVerificationCode(string verificationCode);

    Task<User> CreateUser(User user);
}