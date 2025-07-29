using AISEA.ApiService.SHARED.DTOs.Responses.SubjectVersion;

namespace AISEA.ApiService.SHARED.DTOs.Responses.Syllabus
{
    public class GetSyllabusResponse
    {
        public long Id { get; set; }
        public long SubjectVersionId { get; set; }
        public long SubjectId { get; set; }
        public string SubjectName { get; set; } = null!;
        public string SubjectCode { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // Subject Version Information
        public string? VersionCode { get; set; }
        public string? VersionName { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
    }
}
