using AISEA.ApiService.SHARED.Const.Enums;

namespace AISEA.ApiService.SHARED.DTOs.Requests.User
{
    public class UpdateStudentRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset? DateOfBirth { get; set; }
        public string? AvatarUrl { get; set; }
        public long RoleId { get; set; }
        public EUserStatus Status { get; set; }
        public StudentDataUpdateRequest StudentDataUpdateRequest { get; set; }

    }
    public class StudentDataUpdateRequest
    {
        public DateTimeOffset EnrolledAt { get; set; }
        public bool DoGraduate { get; set; }
        public string? CareerGoal { get; set; }

    }
    public class UpdateStaffRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset? DateOfBirth { get; set; }
        public string? AvatarUrl { get; set; }
        public long RoleId { get; set; }
        public EUserStatus Status { get; set; }
        public StaffDataUpdateRequest StaffDataUpdateRequest { get; set; }

    }
    public class StaffDataUpdateRequest
    {
        public string Campus { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
        public DateTimeOffset? StartWorkAt { get; set; }
        public DateTimeOffset? EndWorkAt { get; set; }
    }
}