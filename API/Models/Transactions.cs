namespace API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

[Table("Transactions")]
public class Transactions
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey("Users")]
    public string? UserId { get; set; }

    public virtual Users? Users { get; set; }

    [ForeignKey("MagicBag")]
    public Guid? MagicBagId { get; set; }

    public virtual MagicBag? MagicBag { get; set; }

    public DateTime TransactionDate { get; set; }

    [Column(TypeName = "double(12,2)")]
    public double Amount { get; set; }
    
    public string? Status { get; set; } = "Pending";

    public DateTime PickUpdateDate { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
}