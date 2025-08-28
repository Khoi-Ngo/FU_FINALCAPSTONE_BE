namespace AISEA.ApiService.SHARED.DTOs.Requests.SystemProfile
{
    public class UpdateComboOrCurriRequest
    {
        public string? RegisteredComboCode { get; set; }
        public required string CurriculumCode { get; set; }
    }
}