namespace AISEA.ApiService.SHARED.DTOs.Responses.MarkReport;

public class TranscriptItemResponse
{
    public string SubjectCode { get; set; }
    public string SubjectVersionCode { get; set; }
    public string? Name { get; set; }
    public bool IsPassed { get; set; }
    public int? Credits { get; set; }
    public double AvgScore { get; set; }
}