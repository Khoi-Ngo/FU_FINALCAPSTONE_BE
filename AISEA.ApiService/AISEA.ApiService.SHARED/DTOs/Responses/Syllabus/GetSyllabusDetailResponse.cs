namespace AISEA.ApiService.SHARED.DTOs.Responses.Syllabus
{
    public class GetSyllabusDetailResponse
    {
        public long Id { get; set; }
        public long SubjectId { get; set; }
        public string SubjectName { get; set; } = null!;
        public string SubjectCode { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        public List<SyllabusAssessmentResponse> Assessments { get; set; } = new();
        public List<SyllabusLearningMaterialResponse> LearningMaterials { get; set; } = new();
        public List<SyllabusLearningOutcomeResponse> LearningOutcomes { get; set; } = new();
        public List<SyllabusSessionResponse> Sessions { get; set; } = new();
    }

    public class SyllabusAssessmentResponse
    {
        public long Id { get; set; }
        public string Category { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal Weight { get; set; }
        public string? CompletionCriteria { get; set; }
        public int? Duration { get; set; }
        public string? QuestionType { get; set; }
    }

    public class SyllabusLearningMaterialResponse
    {
        public long Id { get; set; }
        public string MaterialName { get; set; } = null!;
        public string? AuthorName { get; set; }
        public DateTimeOffset? PublishedDate { get; set; }
        public string? Description { get; set; }
        public string? FilepathOrUrl { get; set; }
    }

    public class SyllabusLearningOutcomeResponse
    {
        public long Id { get; set; }
        public string OutcomeCode { get; set; } = null!;
        public string Description { get; set; } = null!;
    }

    public class SyllabusSessionResponse
    {
        public long Id { get; set; }
        public int SessionNumber { get; set; }
        public string Topic { get; set; } = null!;
        public string? Mission { get; set; }
        public List<string> LearningOutcomeCodes { get; set; } = new();
    }
}