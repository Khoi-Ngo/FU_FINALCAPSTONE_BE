using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;

namespace AISEA.ApiService.SHARED.DTOs.Requests.Combo
{
    public class ComboSearchRequest : PaginationRequest
    {
        public string? Search { get; set; }
        public long? ProgramId { get; set; }
        public int? SemesterNumber { get; set; }
        public string? DifficultyLevel { get; set; }
        public bool? IsAvailable { get; set; }
        public string? SortBy { get; set; } = "ComboName";
        public string? SortOrder { get; set; } = "asc";
    }
}