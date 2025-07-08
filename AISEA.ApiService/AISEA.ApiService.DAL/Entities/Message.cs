using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.DAL.Abstract;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Entities;


[Table("Message")]
public partial class Message
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column(TypeName = "text")]
    public string Content { get; set; } = null!;

    public long SenderId { get; set; }

    public long AdvisorySession1to1Id { get; set; }

    [ForeignKey("AdvisorySession1to1Id")]
    [InverseProperty("Messages")]
    public virtual AdvisorySession1to1 AdvisorySession1to1 { get; set; } = null!;

    [ForeignKey("SenderId")]
    [InverseProperty("Messages")]
    public virtual User Sender { get; set; } = null!;
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
}
