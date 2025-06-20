using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.DAL.Abstract;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Entities;

[Table("Notification")]
public partial class Notification : BaseEntity
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    public long UserId { get; set; }

    [StringLength(500)]
    public string Content { get; set; } = null!;

    public bool IsRead { get; set; } = false;

    [StringLength(255)]
    public string? LinkUrl { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Notifications")]
    public virtual User User { get; set; } = null!;
}