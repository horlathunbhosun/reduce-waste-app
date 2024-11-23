using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class UserRepository
{
    private readonly ApplicationDbContext _context;
    
    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }       
        
    public async Task<User?> FindUserByPhoneNumber(string phoneNumber)
    {
        return await _context.User.FirstOrDefaultAsync(p => p.PhoneNumber == phoneNumber);
    }

    public async Task<User?> FindUserByVerificationCode(string verificationCode)
    {
        return await _context.User.FirstOrDefaultAsync(p => p.VerificationCode == verificationCode);
    }

    public async Task<User?> UserByEmail(string email)
    {
        return await _context.User.FirstOrDefaultAsync(e => e.Email == email);
    }

    public async Task<User?> UserById(int id)
    {
        return await _context.User.FindAsync(id);
    }

    public async Task<User> CreateUser(User user)
    {
        await _context.User.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

}
