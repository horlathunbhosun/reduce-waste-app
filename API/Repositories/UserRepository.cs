using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    
    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }       
        
    public async Task<Users?> FindUserByPhoneNumber(string phoneNumber)
    {
        return await _context.User.FirstOrDefaultAsync(p => p.PhoneNumber == phoneNumber);
    }

    public async Task<Users?> FindUserByVerificationCode(string verificationCode)
    {
        return await _context.User.FirstOrDefaultAsync(p => p.VerificationCode == verificationCode);
    }

    public async Task<Users?> UserByEmail(string email)
    {
        return await _context.User.FirstOrDefaultAsync(e => e.Email == email);
    }

    public async Task<Users?> UserById(int id)
    {
        return await _context.User.FindAsync(id);
    }

    public async Task<Users> CreateUser(Users user)
    {
        // await _context.User.AddAsync(user);
        // await _context.SaveChangesAsync();
        // Console.WriteLine($"User Created Successfully: {user.Id}");
        // await _context.SaveChangesAsync();
        
        // Check if a user with the same UserId already exists
        var existingUser = await _context.User
            .Include(u => u.Partner)
            .FirstOrDefaultAsync(u => u.Id == user.Id);

        if (existingUser != null)
        {
            // Handle the case where the user already exists
            // For example, you can update the existing user or throw an exception
            throw new InvalidOperationException("A user with the same UserId already exists.");
        }

        // Add the new user
        _context.User.Add(user);
        await _context.SaveChangesAsync();
        return user;
        
        
        
    }
    
 

}
