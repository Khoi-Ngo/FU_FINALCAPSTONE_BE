namespace AISEA.ApiService.SHARED.DTOs.Requests.JoinedSubject;

public class SingleImportJoinedSubjectRequest
{
    public string StudentUserName { get; set; }

    public string SubjectCode { get; set; }
    public string SubjectVersionCode { get; set; }
    public string SemesterName { get; set; }
}