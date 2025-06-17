using API.Data;
using API.Models;
using API.Repositories.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    private readonly UserManager<Users> _userManager;
    
    public UserRepository(ApplicationDbContext context, UserManager<Users> userManager)
    {
        _context = context;
        _userManager = userManager;
    }       
        
    public async Task<Users?> FindUserByPhoneNumber(string phoneNumber)
    {
        return await _context.Users.FirstOrDefaultAsync(p => p.PhoneNumber == phoneNumber);
    }

    public async Task<Users?> FindUserByVerificationCode(string verificationCode)
    {
        return await _context.Users.FirstOrDefaultAsync(p => p.VerificationCode == verificationCode);
    }

    public async Task<Users?> FindUserByEmail(string email)
    {
        return await _context.Users.Include("Partner").FirstOrDefaultAsync(e => e.Email == email);
    }

    public async Task<Users?> UserById(string id)
    {
        return await _context.Users.FindAsync(id);
    }
    

    public async Task<Users> CreateUser(Users user, string password)
    {
    
       
        var existingUser = await _context.Users
            .Include(u => u.Partner)
            .FirstOrDefaultAsync(u => u.Id == user.Id);

        if (existingUser != null)
        {
            throw new InvalidOperationException("A user with the same UserId already exists.");
        }

        var userCreation = await _userManager.CreateAsync(user, password);
        
        
        if (userCreation.Succeeded)
        {
            var roleCreation = await _userManager.AddToRoleAsync(user, user.UserType!.ToString());

            if (roleCreation.Succeeded)
            {
                return user;
            }
            else
            {
                throw new Exception("Something went wrong while creating the role, try again");
            }
        }
        else
        {
            var errorMessages = userCreation.Errors.Select(e => e.Description).ToList();

            throw new Exception($"Something went wrong creating user: {string.Join(", ", errorMessages)}");
        }
        
    }
    
    public async Task<Users> UpdateUser(Users user)
    {
        // Check if a user with the same UserId already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == user.Id);

        if (existingUser == null)
        {
            // Handle the case where the user does not exist
            throw new InvalidOperationException("The user does not exist.");
        }

        // Update the user
        _context.Entry(existingUser).CurrentValues.SetValues(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public Task<List<Users>> GetAllUsers()
    {
        return _context.Users.Include("Partner").ToListAsync();
    }

    [HttpDelete]
public async Task<IActionResult> DeleteItems([FromBody] DeleteItemsRequest request)
{
    // EF Core 7+
    var deletedCount = await _context.Items
        .Where(x => request.Ids.Contains(x.Id))
        .ExecuteDeleteAsync();
    
    return Ok(new { DeletedCount = deletedCount });
}

public class DeleteItemsRequest
{
    public List<int> Ids { get; set; } = new();
}


    //AsQueryable 
 

}
