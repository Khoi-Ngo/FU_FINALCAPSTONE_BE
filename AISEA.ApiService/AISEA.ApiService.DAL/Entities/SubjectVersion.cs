using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AISEA.ApiService.DAL.Entities
{
    [Table("SubjectVersion")]
    public partial class SubjectVersion
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [ForeignKey("SubjectId")]
        public long SubjectId { get; set; }
        [StringLength(20)]
        public string VersionCode { get; set; } = null!;// Ví dụ: "1.0", "2.0", "2024.1"
        [StringLength(255)]
        public string VersionName { get; set; } = null!;// Ví dụ: "Phiên bản 2024", "Cập nhật mới"
        [Column(TypeName = "text")]
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true; // Phiên bản có đang được sử dụng không
        public bool IsDefault { get; set; } = false; // Phiên bản mặc định cho môn học
        public DateTime EffectiveFrom { get; set; } // Ngày bắt đầu hiệu lực
        public DateTime? EffectiveTo { get; set; } // Ngày kết thúc hiệu lực (null = vô thời hạn)
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        [InverseProperty("SubjectVersion")]
        public virtual ICollection<Syllabus> Syllabi { get; set; } = new List<Syllabus>();

        [InverseProperty("SubjectVersion")]
        public virtual ICollection<SubjectClass> SubjectClasses { get; set; } = new List<SubjectClass>();

        [InverseProperty("SubjectVersion")]
        public virtual ICollection<CurriculumSubject> CurriculumSubjects { get; set; } = new List<CurriculumSubject>();
        
        [ForeignKey("SubjectId")]
        [InverseProperty("SubjectVersions")]
        public virtual Subject Subject { get; set; } = null!;
    }

}