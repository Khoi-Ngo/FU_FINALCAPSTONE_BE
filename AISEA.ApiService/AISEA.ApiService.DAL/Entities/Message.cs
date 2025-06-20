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

    public long ConversationId { get; set; }

    public long SenderId { get; set; }

    [Column(TypeName = "text")]
    public string Content { get; set; } = null!;

    public DateTimeOffset SendAt { get; set; }

    [ForeignKey("ConversationId")]
    [InverseProperty("Messages")]
    public virtual Conversation Conversation { get; set; } = null!;

    [ForeignKey("SenderId")]
    [InverseProperty("Messages")]
    public virtual User Sender { get; set; } = null!;
}