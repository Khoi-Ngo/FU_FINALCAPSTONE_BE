namespace AISEA.ApiService.SHARED.DTOs.Requests.Subject
{
    public class CreateSubjectRequest
    {
        public string SubjectCode { get; set; } = null!;
        public string SubjectName { get; set; } = null!;
        public int Credits { get; set; }
        public string? Description { get; set; }
    }
}