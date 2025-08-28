namespace AISEA.ApiService.SHARED.DTOs.Requests.User;

public class CreateUserRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTimeOffset? DateOfBirth { get; set; }
    public long RoleId { get; set; }

    //case create student
    public StudentProfileData? StudentProfileData { get; set; }
    //case create other user
    public StaffProfileData? StaffProfileData { get; set; }
}
public class StudentProfileData
{
    public DateTimeOffset EnrolledAt { get; set; }
    public string? CareerGoal { get; set; }
    public required long ProgramId { get; set; }
    public string RegisteredComboCode { get; set; }
    public required string CurriculumCode { get; set; }

}
public class StaffProfileData
{
    public string Campus { get; set; }

    public string Department { get; set; }

    public string Position { get; set; }

    public DateTimeOffset? StartWorkAt { get; set; }

    public DateTimeOffset? EndWorkAt { get; set; }
}