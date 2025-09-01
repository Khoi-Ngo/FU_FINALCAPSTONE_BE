namespace AISEA.ApiService.SHARED.DTOs.Responses.Subject;

public class SimpleSubjectResponse
{
    public long Id { get; set; }
    public string SubjectCode { get; set; }
    public string SubjectName { get; set; }
    public int Credits { get; set; }
    public int SemesterNumber { get; set; }
    public string Description { get; set; } = "N/A";
}