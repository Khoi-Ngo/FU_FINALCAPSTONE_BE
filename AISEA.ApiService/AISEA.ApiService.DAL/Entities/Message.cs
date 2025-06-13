using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.DAL.Abstract;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Entities;


[Table("Message")]
public partial class Message : BaseEntity
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column(TypeName = "text")]
    public string Content { get; set; } = null!;

    public long SenderId { get; set; }

    public long ChatSessionId { get; set; }

    [ForeignKey("ChatSessionId")]
    [InverseProperty("Messages")]
    public virtual ChatSession ChatSession { get; set; } = null!;

    [ForeignKey("SenderId")]
    [InverseProperty("Messages")]
    public virtual User Sender { get; set; } = null!;
}
