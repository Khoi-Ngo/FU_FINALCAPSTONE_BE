namespace AISEA.ApiService.SHARED.DTOs.Requests.Syllabus
{
    public class CreateSyllabusLearningMaterialRequest
    {
        public long SyllabusId { get; set; }
        public string MaterialName { get; set; } = null!;
        public string? AuthorName { get; set; }
        public DateTimeOffset? PublishedDate { get; set; }
        public string? Description { get; set; }
        public string? FilepathOrUrl { get; set; }
    }
}