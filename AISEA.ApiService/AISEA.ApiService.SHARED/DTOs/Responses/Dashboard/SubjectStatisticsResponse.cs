namespace AISEA.ApiService.SHARED.DTOs.Responses.Dashboard
{
    public class SubjectStatisticsResponse
    {
        public List<SubjectsByProgramStats> SubjectsByProgram { get; set; } = new();
        public CreditDistribution CreditDistribution { get; set; } = new();
        public SyllabusAvailability SyllabusAvailability { get; set; } = new();
        public List<SubjectVersionStats> TopSubjectsWithMostVersions { get; set; } = new();
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }

    public class SubjectsByProgramStats
    {
        public string ProgramCode { get; set; } = null!;
        public string ProgramName { get; set; } = null!;
        public int SubjectCount { get; set; }
    }

    public class CreditDistribution
    {
        public int OneToTwoCredits { get; set; }
        public int ThreeToFourCredits { get; set; }
        public int FivePlusCredits { get; set; }
    }

    public class SyllabusAvailability
    {
        public int SubjectsWithSyllabus { get; set; }
        public int SubjectsWithoutSyllabus { get; set; }
        public double PercentageWithSyllabus { get; set; }
    }

    public class SubjectVersionStats
    {
        public string SubjectCode { get; set; } = null!;
        public string SubjectName { get; set; } = null!;
        public int VersionCount { get; set; }
    }
}
