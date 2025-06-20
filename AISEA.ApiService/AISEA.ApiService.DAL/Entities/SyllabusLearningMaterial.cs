using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.DAL.Abstract;

namespace AISEA.ApiService.DAL.Entities;

[Table("SyllabusLearningMaterial")]
public partial class SyllabusLearningMaterial : BaseEntity
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    public long SyllabusId { get; set; }

    [StringLength(255)]
    public string MaterialName { get; set; } = null!;

    [StringLength(255)]
    public string? AuthorName { get; set; }

    public DateTimeOffset? PublishedDate { get; set; }

    [StringLength(500)]
    public string? FilepathOrUrl { get; set; }

    [Column(TypeName = "text")]
    public string? Description { get; set; }

    [ForeignKey("SyllabusId")]
    [InverseProperty("SyllabusLearningMaterials")]
    public virtual Syllabus Syllabus { get; set; } = null!;
}