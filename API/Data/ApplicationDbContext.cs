using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class ApplicationDbContext : DbContext
{

    public ApplicationDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
    {
        
    }
    
    public DbSet<Users> Users { get; set; }

    public DbSet<Partner> Partners { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Users>()
            .Property(u => u.UserType)
            .HasConversion<string>();
        
        modelBuilder.Entity<Users>()
            .Property(u => u.Status)
            .HasConversion<string>();
        
        modelBuilder.Entity<Users>()
            .HasIndex(u => u.Email)
            .IsUnique();

        base.OnModelCreating(modelBuilder);
    }
    
  
    
    
    
}