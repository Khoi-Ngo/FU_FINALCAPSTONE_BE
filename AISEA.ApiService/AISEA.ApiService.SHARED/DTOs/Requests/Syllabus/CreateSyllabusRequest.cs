namespace AISEA.ApiService.SHARED.DTOs.Requests.Syllabus
{
    public class CreateSyllabusRequest
    {
        public long SubjectVersionId { get; set; }
        public string Content { get; set; } = null!;
    }
}