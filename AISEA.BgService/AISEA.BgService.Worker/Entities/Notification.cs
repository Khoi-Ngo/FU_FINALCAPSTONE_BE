using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.BgService.Worker.Abstract;

namespace AISEA.BgService.Worker.Entities;

[Table("Notification")]
public partial class Notification : BaseEntity
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [StringLength(255)]
    public string Title { get; set; } = null!;

    [Column(TypeName = "text")]
    public string Content { get; set; } = null!;

    [Column(TypeName = "text")]
    public string? Link { get; set; }

    public bool IsRead { get; set; }

    public long UserId { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Notifications")]
    public virtual User User { get; set; } = null!;
}