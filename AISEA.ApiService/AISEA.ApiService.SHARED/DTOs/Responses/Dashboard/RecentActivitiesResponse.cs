namespace AISEA.ApiService.SHARED.DTOs.Responses.Dashboard
{
    public class RecentActivitiesResponse
    {
        public List<RecentSubject> NewSubjects { get; set; } = new();
        public List<RecentSyllabus> NewlyApprovedSyllabi { get; set; } = new();
        public List<PendingSubject> PendingSubjects { get; set; } = new();
        public List<ExpiringSubjectVersion> ExpiringSoon { get; set; } = new();
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }

    public class RecentSubject
    {
        public long Id { get; set; }
        public string SubjectCode { get; set; } = null!;
        public string SubjectName { get; set; } = null!;
        public int Credits { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class RecentSyllabus
    {
        public long Id { get; set; }
        public string SubjectCode { get; set; } = null!;
        public string SubjectName { get; set; } = null!;
        public string VersionCode { get; set; } = null!;
        public string? ApprovedBy { get; set; }
        public DateTime ApprovedAt { get; set; }
    }

    public class PendingSubject
    {
        public long Id { get; set; }
        public string SubjectCode { get; set; } = null!;
        public string SubjectName { get; set; } = null!;
        public int Credits { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int DaysPending { get; set; }
    }

    public class ExpiringSubjectVersion
    {
        public long Id { get; set; }
        public string SubjectCode { get; set; } = null!;
        public string SubjectName { get; set; } = null!;
        public string VersionCode { get; set; } = null!;
        public DateTime EffectiveTo { get; set; }
        public int DaysUntilExpiry { get; set; }
    }
}
