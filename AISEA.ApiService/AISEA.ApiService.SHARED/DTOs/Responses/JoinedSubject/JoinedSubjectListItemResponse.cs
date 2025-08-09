using AISEA.ApiService.SHARED.Const.Enums;

namespace AISEA.ApiService.SHARED.DTOs.Responses.JoinedSubject;

public class JoinedSubjectListItemResponse
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? GithubRepositoryURL { get; set; }
    public string SubjectCode { get; set; }
    public string SubjectVersionCode { get; set; }
    public string SubjectName { get; set; }
    public string Name { get; set; }
    public string SemesterName { get; set; }
    public bool IsPassed { get; set; } = false;
    public bool IsCompleted { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public int? Credits { get; set; }
    public long StudentProfileId { get; set; }
    public ESemesterStudyBlockType SemesterStudyBlockType { get; set; }

}