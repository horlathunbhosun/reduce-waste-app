namespace API.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

[Table("Feedbacks")]
public class Feedback
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey("Users")]
    public string? UserId { get; set; }

    public virtual Users? Users { get; set; }

    [ForeignKey("Transactions")]
    public Guid? TransactionId { get; set; }

    public virtual Transactions? Transactions { get; set; }

    public int Rating { get; set; }

    public  string? Comment { get; set; }

    public DateTime RateDate { get; set; }
}