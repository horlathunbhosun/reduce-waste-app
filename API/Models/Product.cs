using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models;

[Table("Products")]
public class Product
{
    [Key]
    public Guid Id { get; set; }
    
    public  string? Name { get; set; }
    
    public string? Description { get; set; }
    public virtual MagicBagItem? MagicBagItems { get; set; }

    
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

}