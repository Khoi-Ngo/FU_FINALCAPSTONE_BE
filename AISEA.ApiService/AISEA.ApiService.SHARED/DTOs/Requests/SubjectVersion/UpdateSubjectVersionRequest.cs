namespace AISEA.ApiService.SHARED.DTOs.Requests.SubjectVersion
{
    public class UpdateSubjectVersionRequest
    {
        public string VersionCode { get; set; } = null!;
        public string VersionName { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
    }
}
