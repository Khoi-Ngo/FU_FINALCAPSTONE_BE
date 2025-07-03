namespace AISEA.ApiService.SHARED.DTOs.Responses.Combo
{
    public class GetComboResponse
    {
        public long Id { get; set; }
        public string ComboName { get; set; } = null!;
        public string? ComboDescription { get; set; }
        public int SubjectCount { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class GetComboDetailResponse : GetComboResponse
    {
        public List<ComboSubjectResponse> Subjects { get; set; } = new();
    }

    public class ComboSubjectResponse
    {
        public long SubjectId { get; set; }
        public string SubjectCode { get; set; } = null!;
        public string SubjectName { get; set; } = null!;
        public int Credits { get; set; }
        public string? Description { get; set; }
    }
}