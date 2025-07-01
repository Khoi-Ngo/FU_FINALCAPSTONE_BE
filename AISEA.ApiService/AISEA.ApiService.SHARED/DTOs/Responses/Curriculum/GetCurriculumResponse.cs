namespace AISEA.ApiService.SHARED.DTOs.Responses.Curriculum
{
    public class GetCurriculumResponse
    {
        public long Id { get; set; }
        public long ProgramId { get; set; }
        public string ProgramName { get; set; } = null!;
        public string ProgramCode { get; set; } = null!;
        public string CurriculumCode { get; set; } = null!;
        public string CurriculumName { get; set; } = null!;
        public DateTimeOffset EffectiveDate { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int TotalSubjects { get; set; }
        public int TotalCredits { get; set; }
    }

    public class GetCurriculumDetailResponse : GetCurriculumResponse
    {
        public List<CurriculumSubjectResponse> Subjects { get; set; } = new();
        public List<CurriculumVersionResponse> Versions { get; set; } = new();
    }

    public class CurriculumSubjectResponse
    {
        public long SubjectId { get; set; }
        public string SubjectCode { get; set; } = null!;
        public string SubjectName { get; set; } = null!;
        public int Credits { get; set; }
        public int SemesterNumber { get; set; }
        public bool IsMandatory { get; set; }
        public List<string> Prerequisites { get; set; } = new();
    }

    public class CurriculumVersionResponse
    {
        public long Id { get; set; }
        public string Version { get; set; } = null!;
        public DateTimeOffset EffectiveDate { get; set; }
        public string? ChangeDescription { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = null!;
    }
}