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
    

    public DbSet<Partner> Partners { get; set; }
    
    public DbSet<Product> Products { get; set; }
    
    public DbSet<MagicBag> MagicBags { get; set; }
    
    public DbSet<ProductMagicBagItem> ProductMagicBagItems { get; set; }
    
    public DbSet<Transactions> Transactions { get; set; }
    
    public DbSet<Feedback> Feedbacks { get; set; }
    
    public DbSet<TransactionColumnDefinition> TransactionColumnDefinitions { get; set; }
    
    public DbSet<TransactionAuditLog> TransactionAuditLogs { get; set; }
    
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


        //modelBuilder.Entity<ProductMagicBagItem>(x =>  x.HasKey(p => new {p.MagicBagId, p.ProductId, p.Id}));
        modelBuilder.Entity<ProductMagicBagItem>()
            .HasKey(p => p.Id); // Set Id as the primary key

        modelBuilder.Entity<ProductMagicBagItem>()
            .HasIndex(p => new { p.MagicBagId, p.ProductId })
            .IsUnique();

        modelBuilder.Entity<ProductMagicBagItem>()
            .HasOne(p => p.MagicBag)
            .WithMany(m => m.MagicBagItems)
            .HasForeignKey(p => p.MagicBagId);
        
        modelBuilder.Entity<ProductMagicBagItem>()
            .HasOne(p => p.Products)
            .WithMany(p => p.MagicBagItems)
            .HasForeignKey(p => p.ProductId);
         
        
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
        
        
        modelBuilder.Entity<Transactions>()
            .HasKey(p => p.Id);  // Set Id as the primary key

        modelBuilder.Entity<Transactions>()
            .HasIndex(p => new { p.UserId, p.MagicBagId })
            .IsUnique();

        modelBuilder.Entity<Transactions>()
            .HasOne(p => p.Users)
            .WithMany(m => m.UserTransactions)
            .HasForeignKey(p => p.UserId);

        modelBuilder.Entity<Transactions>()
            .HasIndex(p => p.MagicBagId)
            .IsUnique();

        modelBuilder.Entity<Feedback>()
            .HasKey(p => p.Id);

        modelBuilder.Entity<Feedback>()
            .HasIndex(p => new { p.UserId, p.TransactionId })
            .IsUnique();

        modelBuilder.Entity<Feedback>()
            .HasIndex(p => p.UserId)
            .IsUnique();

        modelBuilder.Entity<Feedback>()
            .HasOne(p => p.Users)
            .WithMany(m => m.UserFeedback)
            .HasForeignKey(p => p.UserId);

        modelBuilder.Entity<Feedback>()
            .HasIndex(p => p.TransactionId)
            .IsUnique();

        // Transaction Audit Log Configuration
        modelBuilder.Entity<TransactionAuditLog>()
            .HasKey(p => p.Id);

        modelBuilder.Entity<TransactionAuditLog>()
            .HasIndex(p => p.TransactionId);

        modelBuilder.Entity<TransactionAuditLog>()
            .HasIndex(p => p.Timestamp);

        modelBuilder.Entity<TransactionAuditLog>()
            .HasIndex(p => p.UserId);

        modelBuilder.Entity<TransactionAuditLog>()
            .HasIndex(p => new { p.TransactionId, p.Timestamp });

        modelBuilder.Entity<TransactionAuditLog>()
            .HasOne(p => p.Transaction)
            .WithMany()
            .HasForeignKey(p => p.TransactionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TransactionAuditLog>()
            .Property(p => p.Operation)
            .IsRequired()
            .HasMaxLength(10);

        modelBuilder.Entity<TransactionAuditLog>()
            .Property(p => p.ChangeSource)
            .HasDefaultValue("APPLICATION")
            .HasMaxLength(50);
        
        
       
    }
    
  
    
    
    
}