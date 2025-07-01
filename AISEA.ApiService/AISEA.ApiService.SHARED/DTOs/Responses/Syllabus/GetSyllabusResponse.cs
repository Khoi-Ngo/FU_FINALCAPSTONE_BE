namespace AISEA.ApiService.SHARED.DTOs.Responses.Syllabus
{
    public class GetSyllabusResponse
    {
        public long Id { get; set; }
        public long SubjectId { get; set; }
        public string SubjectName { get; set; } = null!;
        public string SubjectCode { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}