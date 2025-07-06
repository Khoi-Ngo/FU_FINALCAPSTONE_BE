using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.BgService.Worker.Enums;

namespace AISEA.BgService.Worker.Entities;

[Table("AuditLog")]
public class AuditLog
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    public EAuditLogTag Tag { get; set; }

    [StringLength(20000)]
    public string? Description { get; set; } 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}