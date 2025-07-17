using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISEA.ApiService.SHARED.Const.Enums;

namespace AISEA.ApiService.SHARED.DTOs.Responses.User;

public class GetStudentListResponse
{
    public long Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTimeOffset? DateOfBirth { get; set; }
    public string? AvatarUrl { get; set; }
    public string RoleName { get; set; }
    public EUserStatus Status { get; set; }
    public StudentDataListResponse StudentDataListResponse { get; set; }

}

public class StudentDataListResponse
{
    public long Id { get; set; }
    public DateTimeOffset EnrolledAt { get; set; }
    public bool DoGraduate { get; set; }
    public string? CareerGoal { get; set; }
    public int NumberOfBan { get; set; }

}