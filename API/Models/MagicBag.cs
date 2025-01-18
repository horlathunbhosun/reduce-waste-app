using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models;


[Table("MagicBags")]
public class MagicBag
{
    [Key]
    public Guid Id { get; set; }
    
    public string? Name { get; set; }
    
    public string? Description { get; set; }
    
    [Column(TypeName = "double(12,2)")]
    public double BagPrice { get; set; }
    
    [ForeignKey("Partner")]
    public  int PartnerId { get; set; }
    
    public virtual Partner? Partner { get; set; }
    
    public  List<ProductMagicBagItem>? MagicBagItems { get; set; }
    
    public string? Status { get; set; } = "Active";
    
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Transactions? UserTransaction { get; set; }
}