namespace AISEA.ApiService.SHARED.DTOs.Requests.Curriculum
{
    public class UpdateCurriculumRequest
    {
        public string CurriculumName { get; set; } = null!;
        public DateTimeOffset EffectiveDate { get; set; }
        public string? Description { get; set; }
        public List<CurriculumSubjectRequest>? Subjects { get; set; }
    }
}