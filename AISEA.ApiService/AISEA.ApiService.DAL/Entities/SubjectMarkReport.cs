using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AISEA.ApiService.DAL.Entities
{
    [Table("SubjectMarkReport")]
    public class SubjectMarkReport
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Category { get; set; }
        public double Weight { get; set; }
        public double MinScore { get; set; }
        public string? ScoreUpdatedBy { get; set; }

        // Foreign Key
        [ForeignKey("JoinedSubject")]
        public long JoinedSubjectId { get; set; }

        // Navigation property
        public virtual JoinedSubject JoinedSubject { get; set; } = null!;
    }
}
