namespace AISEA.ApiService.SHARED.DTOs.Requests.SubjectVersion
{
    public class CreateSubjectVersionRequest
    {
        public long SubjectId { get; set; }
        public string VersionCode { get; set; } = null!;
        public string VersionName { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDefault { get; set; } = false;
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
    }
}
