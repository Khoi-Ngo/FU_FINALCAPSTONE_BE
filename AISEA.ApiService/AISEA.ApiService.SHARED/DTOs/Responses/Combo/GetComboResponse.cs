namespace AISEA.ApiService.SHARED.DTOs.Responses.Combo
{
    public class GetComboResponse
    {
        public long Id { get; set; }
        public string ComboName { get; set; } = null!;
        public string? ComboDescription { get; set; }
        public int SemesterNumber { get; set; }
        public long ProgramId { get; set; }
        public string ProgramName { get; set; } = null!;
        public string DifficultyLevel { get; set; } = null!;
        public int MaxStudents { get; set; }
        public int CurrentEnrollment { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<ComboSubjectResponse> Subjects { get; set; } = new();
    }

    public class GetComboDetailResponse : GetComboResponse
    {
        public List<ComboPrerequisiteResponse> Prerequisites { get; set; } = new();
        public List<StudentEnrollmentResponse> Enrollments { get; set; } = new();
        public ComboStatisticsResponse Statistics { get; set; } = new();
    }

    public class ComboSubjectResponse
    {
        public long SubjectId { get; set; }
        public string SubjectCode { get; set; } = null!;
        public string SubjectName { get; set; } = null!;
        public int Credits { get; set; }
        public string? Description { get; set; }
    }

    public class ComboPrerequisiteResponse
    {
        public long SubjectId { get; set; }
        public string SubjectCode { get; set; } = null!;
        public string SubjectName { get; set; } = null!;
        public bool IsRequired { get; set; }
    }

    public class StudentEnrollmentResponse
    {
        public long StudentId { get; set; }
        public string StudentCode { get; set; } = null!;
        public string StudentName { get; set; } = null!;
        public DateTime EnrolledAt { get; set; }
        public string? Notes { get; set; }
        public string Status { get; set; } = null!;
    }

    public class ComboStatisticsResponse
    {
        public int TotalCredits { get; set; }
        public double AverageGPA { get; set; }
        public int CompletionRate { get; set; }
        public Dictionary<string, int> GradeDistribution { get; set; } = new();
    }

    public class ComboAvailabilityResponse
    {
        public long ComboId { get; set; }
        public string ComboName { get; set; } = null!;
        public bool IsAvailable { get; set; }
        public int AvailableSlots { get; set; }
        public List<string> UnavailableReasons { get; set; } = new();
        public List<string> MissingPrerequisites { get; set; } = new();
    }
}