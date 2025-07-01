namespace AISEA.ApiService.SHARED.DTOs.Requests.Syllabus
{
    public class CreateSyllabusSessionRequest
    {
        public long SyllabusId { get; set; }
        public int SessionNumber { get; set; }
        public string Topic { get; set; } = null!;
        public string? Mission { get; set; }
    }
}