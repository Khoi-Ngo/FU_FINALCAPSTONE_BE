using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.DAL.Abstract;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Entities;

[Table("ChatSession")]
public partial class ChatSession : BaseEntity
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [StringLength(255)]
    public string Title { get; set; } = null!;

    public long StaffId { get; set; }

    [StringLength(255)]
    public string Status { get; set; } = null!;

    public long Type { get; set; }

    [InverseProperty("ChatSession")]
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    [ForeignKey("StaffId")]
    [InverseProperty("ChatSessions")]
    public virtual User Staff { get; set; } = null!;
}
