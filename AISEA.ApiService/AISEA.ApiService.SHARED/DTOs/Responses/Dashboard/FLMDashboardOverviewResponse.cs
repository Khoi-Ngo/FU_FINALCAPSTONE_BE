namespace AISEA.ApiService.SHARED.DTOs.Responses.Dashboard
{
    public class FLMDashboardOverviewResponse
    {
        public OverviewSummary Summary { get; set; } = new();
        public ApprovalStatusDistribution ApprovalDistribution { get; set; } = new();
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }

    public class OverviewSummary
    {
        public int TotalSubjects { get; set; }
        public int TotalCurricula { get; set; }
        public int ActiveSubjectVersions { get; set; }
        public int TotalSyllabi { get; set; }
    }

    public class ApprovalStatusDistribution
    {
        public EntityApprovalStats Subjects { get; set; } = new();
        public EntityApprovalStats Curricula { get; set; } = new();
        public EntityApprovalStats Syllabi { get; set; } = new();
    }

    public class EntityApprovalStats
    {
        public int Pending { get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }
        public int Total => Pending + Approved + Rejected;
    }
}
