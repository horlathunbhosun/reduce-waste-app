using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models;


[Table("MagicBagItems")]
public class MagicBagItem
{
    [Key]
    public Guid Id { get; set; }
    
    public virtual MagicBag? MagicBag { get; set; }
    
    public List<Product>? Products { get; set; }
    
    public int Quantity { get; set; }
    
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

}