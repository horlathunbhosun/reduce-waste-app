using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models;


[Table("ProductMagicBagItems")]
public class ProductMagicBagItem
{
    [Key]
    public Guid Id { get; set; }
    
    [ForeignKey("MagicBag")]
    public Guid? MagicBagId { get; set; }

    public virtual MagicBag? MagicBag { get; set; }
    
    [ForeignKey("Product")]
    public Guid? ProductId { get; set; }
    public virtual Product? Products { get; set; }
    
    public int Quantity { get; set; }
    
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

}