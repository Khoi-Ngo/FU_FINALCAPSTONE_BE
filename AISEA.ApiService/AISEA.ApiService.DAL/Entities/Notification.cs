using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.DAL.Abstract;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Entities;

[Table("Notification")]
public partial class Notification
{
    [Key]
    [Column("id")]
    public long Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string? Link { get; set; }
    public bool IsRead { get; set; } = false;
    public long UserId { get; set; }
    [ForeignKey("UserId")]
    [InverseProperty("Notifications")]
    public virtual User User { get; set; } = null!;
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
}