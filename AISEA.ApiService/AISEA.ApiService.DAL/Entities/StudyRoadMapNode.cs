using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AISEA.ApiService.DAL.Entities;

public class StudyRoadMapNode
{
    [Key]
    [Column("id")]
    public long Id { get; set; }
    public string SubjectCode { get; set; }
    public int SemesterNumber { get; set; }
    public string? Name { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string? URL { get; set; }

    // Foreign key to StudyRoadMap
    public long StudyRoadMapId { get; set; }

    [ForeignKey(nameof(StudyRoadMapId))]
    public virtual StudyRoadMap StudyRoadMap { get; set; }

}