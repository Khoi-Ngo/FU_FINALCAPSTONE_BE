namespace AISEA.ApiService.SHARED.DTOs.Responses.JoinedSubject
{
    public class JoinedSubjectSyllabusResponse
    {
        public long JoinedSubjectId { get; set; }
        public string SubjectCode { get; set; } = null!;
        public string SubjectVersionCode { get; set; } = null!;
        public string SubjectName { get; set; } = null!;
        public long? SyllabusId { get; set; }
        public bool HasSyllabus { get; set; }
        public string? Message { get; set; }
    }
}
