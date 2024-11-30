using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class ApplicationDbContext : IdentityDbContext<Users>
{

    public ApplicationDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
    {
        
    }
    
    public DbSet<Users> Users { get; set; }

    public DbSet<Partner> Partners { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        List<IdentityRole> roles = new List<IdentityRole>
        {
            new IdentityRole {Name = "Admin", NormalizedName = "ADMIN"},
            new IdentityRole {Name = "Partner", NormalizedName = "PARTNER"},
            new IdentityRole {Name = "User", NormalizedName = "USER"}
        };

        modelBuilder.Entity<IdentityRole>().HasData(roles);
        
        modelBuilder.Entity<Users>()
            .HasOne(u => u.Partner)
            .WithOne(p => p.User)
            .HasForeignKey<Partner>(p => p.UserId);
        
        
        modelBuilder.Entity<Users>()
            .Property(u => u.UserType)
            .HasConversion<string>();
        
        modelBuilder.Entity<Users>()
            .Property(u => u.Status)
            .HasConversion<string>();
        
        modelBuilder.Entity<Users>()
            .HasIndex(u => u.Email)
            .IsUnique();

        
       
    }
    
  
    
    
    
}