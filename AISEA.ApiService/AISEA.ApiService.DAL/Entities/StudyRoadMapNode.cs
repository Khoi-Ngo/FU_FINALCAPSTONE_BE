using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AISEA.ApiService.DAL.Entities;

public class StudyRoadMapNode
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    public string SubjectCode { get; set; }
    public int? SemesterNumber { get; set; }
    public bool IsInternalSubjectData { get; set; }
    public string? SubjectName { get; set; }
    public string? Description { get; set; }
    public long StudyRoadMapId { get; set; }




    [ForeignKey(nameof(StudyRoadMapId))]
    public virtual StudyRoadMap StudyRoadMap { get; set; }

    public virtual ICollection<StudyRoadMapNodeLink> Prerequisites { get; set; } = new List<StudyRoadMapNodeLink>();
    public virtual ICollection<StudyRoadMapNodeLink> Dependents { get; set; } = new List<StudyRoadMapNodeLink>();

}