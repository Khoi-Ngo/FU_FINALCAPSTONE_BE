using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.SHARED.Const.Enums;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Entities;

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
    public virtual ICollection<StaffProfile> StaffProfiles { get; set; } = new List<StaffProfile>();

    [InverseProperty("User")]
    public virtual ICollection<StudentProfile> StudentProfiles { get; set; } = new List<StudentProfile>();
}