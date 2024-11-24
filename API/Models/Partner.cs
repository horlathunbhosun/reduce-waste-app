using System.ComponentModel.DataAnnotations;

namespace API.Models;

public class Partner
{
   
    [Key]
    public int Id { get; set; }

    public Guid Uuid { get; set; } = System.Guid.NewGuid();
    public int? UserId { get; set; }

    //Navigation Property
    public Users? User { get; set; }

    public int BusinessNumber { get; set; }

    public string Logo { get; set; } = string.Empty;

    public string Address   { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
 
}