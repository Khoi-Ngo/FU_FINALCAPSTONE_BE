namespace AISEA.ApiService.SHARED.DTOs.Requests.Curriculum
{
    public class CreateCurriculumRequest
    {
        public long ProgramId { get; set; }
        public string CurriculumCode { get; set; } = null!;
        public string CurriculumName { get; set; } = null!;
        public DateTimeOffset EffectiveDate { get; set; }
        public string? Description { get; set; }
        public List<CurriculumSubjectRequest>? Subjects { get; set; }
    }

    public class CurriculumSubjectRequest
    {
        public long SubjectId { get; set; }
        public int SemesterNumber { get; set; }
        public bool IsMandatory { get; set; }
    }
}