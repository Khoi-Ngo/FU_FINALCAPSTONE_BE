using AISEA.ApiService.SHARED.Const.Enums;

namespace AISEA.ApiService.SHARED.DTOs.Requests.JoinedSubject;

public class ImportJoinedSubjectsForOneStudentRequest
{
    public string StudentUserName { get; set; }
    public HashSet<ImportJoinedSubjects_Data> SubjectsData { get; set; }
}

public class ImportJoinedSubjects_Data
{
    public string SubjectCode { get; set; }
    public string SubjectVersionCode { get; set; }
    public long SemesterId { get; set; }
    public ESemesterStudyBlockType SemesterStudyBlockType { get; set; }


    public override bool Equals(object obj)
    {
        if (obj is ImportJoinedSubjects_Data other)
        {
            return string.Equals(SubjectCode, other.SubjectCode, StringComparison.OrdinalIgnoreCase)
                && SemesterId == other.SemesterId
                && SemesterStudyBlockType == other.SemesterStudyBlockType;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            SubjectCode?.ToLowerInvariant(),
            SemesterId,
            SemesterStudyBlockType
        );
    }
}
