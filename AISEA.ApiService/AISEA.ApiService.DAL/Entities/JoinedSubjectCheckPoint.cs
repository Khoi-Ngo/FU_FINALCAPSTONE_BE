using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AISEA.ApiService.DAL.Entities
{
    [Table("JoinedSubjectCheckPoint")]
    public class JoinedSubjectCheckPoint
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        public string Title { get; set; }
        public string? Content { get; set; }
        public string? Note { get; set; }
        public bool IsCompleted { get; set; } = false;

        public string? Link1 { get; set; }
        public string? Link2 { get; set; }
        public string? Link3 { get; set; }
        public string? Link4 { get; set; }
        public string? Link5 { get; set; }

        public DateTime Deadline { get; set; }


        // Foreign key
        [ForeignKey("JoinedSubject")]
        public long JoinedSubjectId { get; set; }

        // Navigation property
        public virtual JoinedSubject JoinedSubject { get; set; } = null!;
    }
}
