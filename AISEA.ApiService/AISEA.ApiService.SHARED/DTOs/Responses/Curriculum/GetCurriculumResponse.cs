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
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class GetCurriculumDetailResponse : GetCurriculumResponse
    {
        public List<CurriculumSubjectResponse> Subjects { get; set; } = new();
    }

    public class CurriculumSubjectResponse
    {
        public long SubjectId { get; set; }
        public long SubjectVersionId { get; set; }
        public string SubjectCode { get; set; } = null!;
        public string SubjectName { get; set; } = null!;
        public string VersionCode { get; set; } = null!;
        public string VersionName { get; set; } = null!;
        public int Credits { get; set; }
        public int SemesterNumber { get; set; }
        public bool IsMandatory { get; set; }
        public string? Description { get; set; }
    }
}