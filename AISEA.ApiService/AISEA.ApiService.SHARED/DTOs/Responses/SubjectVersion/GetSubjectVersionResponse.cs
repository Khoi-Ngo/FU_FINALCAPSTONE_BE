using AISEA.ApiService.SHARED.DTOs.Responses.Subject;

namespace AISEA.ApiService.SHARED.DTOs.Responses.SubjectVersion
{
    public class GetSubjectVersionResponse
    {
        public long Id { get; set; }
        public long SubjectId { get; set; }
        public string VersionCode { get; set; } = null!;
        public string VersionName { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public GetSubjectResponse? Subject { get; set; }
    }
}
