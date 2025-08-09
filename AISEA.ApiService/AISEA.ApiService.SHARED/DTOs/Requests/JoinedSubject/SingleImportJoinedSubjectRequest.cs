using AISEA.ApiService.SHARED.Const.Enums;

namespace AISEA.ApiService.SHARED.DTOs.Requests.JoinedSubject;

public class SingleImportJoinedSubjectRequest
{
    public string StudentUserName { get; set; }

    public string SubjectCode { get; set; }
    public string SubjectVersionCode { get; set; }
    public long SemesterId { get; set; }
    public string SubjectName { get; set; }
    public ESemesterStudyBlockType SemesterStudyBlockType { get; set; }

}