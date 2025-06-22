using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.BgService.Worker.Abstract;

namespace AISEA.BgService.Worker.Entities;

[Table("StaffProfile")]
public partial class StaffProfile : BaseEntity
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [StringLength(255)]
    public string Campus { get; set; } = null!;

    [StringLength(255)]
    public string Department { get; set; } = null!;

    [StringLength(255)]
    public string Position { get; set; } = null!;


    public DateTimeOffset? StartWorkAt { get; set; }

    public DateTimeOffset? EndWorkAt { get; set; }

    public long UserId { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("StaffProfile")]
    public virtual User User { get; set; } = null!;


    [InverseProperty("Staff")]
    public virtual ICollection<AdvisorySession1to1> AdvisorySessions1to1 { get; set; } = new List<AdvisorySession1to1>();
}