namespace API.Models;

public class Partner
{
   
    public int Id { get; set; }

    public int? UserId { get; set; }

    //Navigation Property
    public User? User { get; set; }

    public int BusinessNumber { get; set; }

    public string Logo { get; set; } = string.Empty;

    public string Address   { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
 
}