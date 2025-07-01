namespace AISEA.ApiService.SHARED.DTOs.Requests.Combo
{
    public class CreateComboRequest
    {
        public string ComboName { get; set; } = null!;
        public string? ComboDescription { get; set; }
        public int SemesterNumber { get; set; }
        public long ProgramId { get; set; }
        public string DifficultyLevel { get; set; } = null!; // Easy, Medium, Hard
        public int MaxStudents { get; set; }
        public List<long> SubjectIds { get; set; } = new();
        public List<ComboPrerequisiteRequest>? Prerequisites { get; set; }
    }

    public class ComboPrerequisiteRequest
    {
        public long SubjectId { get; set; }
        public bool IsRequired { get; set; } = true;
    }
}