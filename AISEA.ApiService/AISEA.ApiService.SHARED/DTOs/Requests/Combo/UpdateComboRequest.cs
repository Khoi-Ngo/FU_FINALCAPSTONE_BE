namespace AISEA.ApiService.SHARED.DTOs.Requests.Combo
{
    public class UpdateComboRequest
    {
        public string ComboName { get; set; } = null!;
        public string? ComboDescription { get; set; }
    }
}