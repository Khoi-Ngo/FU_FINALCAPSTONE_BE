namespace AISEA.ApiService.SHARED.DTOs.Requests.Syllabus
{
    public class CreateSyllabusAssessmentRequest
    {
        public long SyllabusId { get; set; }
        public string Category { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal Weight { get; set; }
        public string? CompletionCriteria { get; set; }
        public int? Duration { get; set; }
        public string? QuestionType { get; set; }
    }
}