using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AISEA.ApiService.DAL.Entities
{
    [Table("StudyRoadMap")]
    public class StudyRoadMap
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        public string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public long StudentProfileId { get; set; }

        [ForeignKey(nameof(StudentProfileId))]
        public virtual StudentProfile StudentProfile { get; set; }
        public virtual ICollection<StudyRoadMapNode> Nodes { get; set; } = new List<StudyRoadMapNode>();
    }
}
