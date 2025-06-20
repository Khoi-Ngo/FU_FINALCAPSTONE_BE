using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.DAL.Abstract;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Entities;

[Table("StudentProfile")]
public partial class StudentProfile : BaseEntity
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    public long UserId { get; set; }

    public DateTimeOffset EnrolledAt { get; set; }

    public bool DoGraduate { get; set; }

    [Column("GPA")]
    public double Gpa { get; set; }

    [StringLength(255)]
    public string Status { get; set; } = null!;

    [Column(TypeName = "text")]
    public string CareerGoal { get; set; } = null!;

    public long TotalCreditsEarnt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("StudentProfiles")]
    public virtual User User { get; set; } = null!;

    [InverseProperty("Student")]
    public virtual ICollection<AdvisorySession1to1> AdvisorySessions1to1 { get; set; } = new List<AdvisorySession1to1>();
}