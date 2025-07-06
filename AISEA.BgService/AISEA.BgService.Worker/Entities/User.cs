using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.BgService.Worker.Abstract;
using AISEA.BgService.Worker.Enums;
using Microsoft.EntityFrameworkCore;

namespace AISEA.BgService.Worker.Entities;

[Table("User")]
[Index("Email", Name = "user_email_unique", IsUnique = true)]
[Index("Username", Name = "user_username_unique", IsUnique = true)]
public partial class User : BaseEntity
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [StringLength(255)]
    public string Username { get; set; } = null!;

    [StringLength(255)]
    public string Password { get; set; } = null!;

    [StringLength(255)]
    public string Email { get; set; } = null!;

    [StringLength(255)]
    public string FirstName { get; set; } = null!;

    [StringLength(255)]
    public string LastName { get; set; } = null!;

    public DateTimeOffset? DateOfBirth { get; set; }

    [StringLength(255)]
    public string? AvatarUrl { get; set; }
    public EUserStatus Status { get; set; } = EUserStatus.ACTIVE;
    public long RoleId { get; set; }

    [InverseProperty("Sender")]
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    [InverseProperty("User")]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    [ForeignKey("RoleId")]
    [InverseProperty("Users")]
    public virtual Role Role { get; set; } = null!;

   [InverseProperty("User")]
    public virtual StaffProfile? StaffProfile { get; set; }

    [InverseProperty("User")]
    public virtual StudentProfile? StudentProfile { get; set; }
}