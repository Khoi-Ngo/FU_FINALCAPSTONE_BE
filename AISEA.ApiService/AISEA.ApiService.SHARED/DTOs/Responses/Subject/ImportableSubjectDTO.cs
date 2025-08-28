namespace AISEA.ApiService.SHARED.DTOs.Responses.Subject
{
    public class ImportableSubjectDTO
    {
        // Subject info
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public int Credits { get; set; }
        public string? Description { get; set; }

        // Flattened related data
        public List<string> PrerequisiteSubjectCodes { get; set; } = new();
        public List<string> Versions { get; set; } = new();
        public List<string> CurriculumCodes { get; set; } = new();
        public List<string> ComboNames { get; set; } = new();
    }
}