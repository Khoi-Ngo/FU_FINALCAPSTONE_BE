namespace AISEA.ApiService.SHARED.DTOs.Responses.Dashboard
{
    public class CurriculaStatisticsResponse
    {
        public List<CurriculaByProgramStats> CurriculaByProgram { get; set; } = new();
        public AverageSubjectsPerCurriculum AverageSubjects { get; set; } = new();
        public CurriculumSizeDistribution SizeDistribution { get; set; } = new();
        public SemesterCompleteness SemesterCompleteness { get; set; } = new();
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }

    public class CurriculaByProgramStats
    {
        public string ProgramCode { get; set; } = null!;
        public string ProgramName { get; set; } = null!;
        public int CurriculumCount { get; set; }
    }

    public class AverageSubjectsPerCurriculum
    {
        public double Average { get; set; }
        public int MinSubjects { get; set; }
        public int MaxSubjects { get; set; }
    }

    public class CurriculumSizeDistribution
    {
        public int LessThan30Subjects { get; set; }
        public int Between30And50Subjects { get; set; }
        public int MoreThan50Subjects { get; set; }
    }

    public class SemesterCompleteness
    {
        public int CurriculaWithFullEightSemesters { get; set; }
        public int TotalCurricula { get; set; }
        public double PercentageComplete { get; set; }
    }
}
