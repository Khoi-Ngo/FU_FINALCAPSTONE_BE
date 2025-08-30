namespace AISEA.ApiService.SHARED.DTOs.Requests.MarkReport
{
    public class CreateMarkReportRequest
    {
        public string Category { get; set; }
        public double Weight { get; set; }
        public double MinScore { get; set; }
        public double Score { get; set; }

    }
}