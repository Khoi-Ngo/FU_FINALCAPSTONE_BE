using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;

namespace AISEA.ApiService.SHARED.DTOs.Requests.Curriculum
{
    public class CurriculumSearchRequest : PaginationRequest
    {
        public string? Search { get; set; }
        public long? ProgramId { get; set; }
        public DateTimeOffset? EffectiveDateFrom { get; set; }
        public DateTimeOffset? EffectiveDateTo { get; set; }
        public bool? IsActive { get; set; }
        public string? SortBy { get; set; } = "CurriculumName";
        public string? SortOrder { get; set; } = "asc";
    }
}