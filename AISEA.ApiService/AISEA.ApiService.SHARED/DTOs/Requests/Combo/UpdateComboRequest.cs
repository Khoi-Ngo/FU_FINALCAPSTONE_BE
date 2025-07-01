namespace AISEA.ApiService.SHARED.DTOs.Requests.Combo
{
    public class UpdateComboRequest
    {
        public string ComboName { get; set; } = null!;
        public string? ComboDescription { get; set; }
        public int SemesterNumber { get; set; }
        public string DifficultyLevel { get; set; } = null!;
        public int MaxStudents { get; set; }
        public List<long> SubjectIds { get; set; } = new();
        public List<ComboPrerequisiteRequest>? Prerequisites { get; set; }
    }
}