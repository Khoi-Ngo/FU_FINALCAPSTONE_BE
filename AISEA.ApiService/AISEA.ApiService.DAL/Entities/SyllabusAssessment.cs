using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.DAL.Abstract;

namespace AISEA.ApiService.DAL.Entities;

[Table("SyllabusAssessment")]
public partial class SyllabusAssessment : BaseEntity
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    public long SyllabusId { get; set; }

    [StringLength(100)]
    public string Category { get; set; } = null!; // Assignment, Quiz, Final Exam

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal Weight { get; set; }

    [Column(TypeName = "text")]
    public string? CompletionCriteria { get; set; }

    public int? Duration { get; set; } // in minutes

    [StringLength(255)]
    public string? QuestionType { get; set; } // essay, multiple-choice, practical exam

    [ForeignKey("SyllabusId")]
    [InverseProperty("SyllabusAssessments")]
    public virtual Syllabus Syllabus { get; set; } = null!;
}