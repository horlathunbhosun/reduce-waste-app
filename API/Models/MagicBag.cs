using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models;


[Table("MagicBags")]
public class MagicBag
{
    [Key]
    public Guid Id { get; set; }
    [Column(TypeName = "double(12,2")]
    public double BagPrice { get; set; }
    
    [ForeignKey("Partner")]
    public int PartnerId { get; set; }
    
    public virtual Partner? Partner { get; set; }
    
    private  List<MagicBagItem>? MagicBagItems { get; set; }
    
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}