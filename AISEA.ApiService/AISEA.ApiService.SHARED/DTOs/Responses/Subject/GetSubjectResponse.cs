namespace AISEA.ApiService.SHARED.DTOs.Responses.Subject
{
    public class GetSubjectResponse
    {
        public long Id { get; set; }
        public string SubjectCode { get; set; } = null!;
        public string SubjectName { get; set; } = null!;
        public int Credits { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<GetSubjectResponse>? Prerequisites { get; set; }

    }
}