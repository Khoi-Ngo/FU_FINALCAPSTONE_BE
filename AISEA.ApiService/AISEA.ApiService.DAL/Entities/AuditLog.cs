using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AISEA.ApiService.DAL.Entities;

[Table("AuditLog")]
public class AuditLog
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    public string Tag { get; set; }
    public bool IsSuccessAction { get; set; } = true;

    [StringLength(20000)]
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? UserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public long? RoleId { get; set; }
    public string? Email { get; set; }
    public string? IPAddress { get; set; }
    public string? UserAgent { get; set; }
    public long? UserId { get; set; }
}