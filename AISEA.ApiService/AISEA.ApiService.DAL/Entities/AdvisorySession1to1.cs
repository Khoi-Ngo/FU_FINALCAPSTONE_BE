using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.SHARED.Const.Enums;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Entities;

[Table("AdvisorySession1to1")]
public partial class AdvisorySession1to1 : BaseEntity
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [StringLength(255)]
    public string Title { get; set; } = null!;

    public long StaffId { get; set; }

    [StringLength(255)]
    public EAdvisorySessionStatus Status { get; set; } = EAdvisorySessionStatus.ACTIVE;

    public EAdvisorySessionType Type { get; set; }

    public long StudentId { get; set; }

    [InverseProperty("AdvisorySession1to1")]
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    [ForeignKey("StaffId")]
    [InverseProperty("AdvisorySessions1to1")]
    public virtual StaffProfile Staff { get; set; } = null!;

    [ForeignKey("StudentId")]
    [InverseProperty("AdvisorySessions1to1")]
    public virtual StudentProfile Student { get; set; } = null!;
}