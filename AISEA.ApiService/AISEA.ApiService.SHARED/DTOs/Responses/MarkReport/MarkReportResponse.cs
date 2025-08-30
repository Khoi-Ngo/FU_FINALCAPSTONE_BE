namespace AISEA.ApiService.SHARED.DTOs.Responses.MarkReport
{
    public class MarkReportResponse
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Category { get; set; }
        public double Weight { get; set; }
        public double MinScore { get; set; }
        public string? ScoreUpdatedBy { get; set; }
        public long JoinedSubjectId { get; set; }
        public double Score { get; set; }


    }
}