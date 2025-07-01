namespace AISEA.ApiService.SHARED.DTOs.Requests.Syllabus
{
    public class CreateSyllabusLearningOutcomeRequest
    {
        public long SyllabusId { get; set; }
        public string OutcomeCode { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}