namespace AISEA.ApiService.SHARED.DTOs.Requests.Syllabus
{
    public class UpdateSyllabusSessionRequest
    {
        public int SessionNumber { get; set; }
        public string Topic { get; set; } = null!;
        public string? Mission { get; set; }
    }
}
