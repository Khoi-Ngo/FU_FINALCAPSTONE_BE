namespace AISEA.ApiService.SHARED.DTOs.Requests.Curriculum
{
    public class CreateCurriculumRequest
    {
        public long ProgramId { get; set; }
        public string CurriculumCode { get; set; } = null!;
        public string CurriculumName { get; set; } = null!;
        public DateTimeOffset EffectiveDate { get; set; }
    }
}