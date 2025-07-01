namespace AISEA.ApiService.SHARED.DTOs.Requests.Combo
{
    public class CreateComboRequest
    {
        public string ComboName { get; set; } = null!;
        public string? ComboDescription { get; set; }
        public List<long> SubjectIds { get; set; } = new();
    }
}