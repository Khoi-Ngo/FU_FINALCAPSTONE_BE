namespace AISEA.ApiService.SHARED.DTOs.Requests.User;

public class BulkCreateStaffByRoleRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTimeOffset? DateOfBirth { get; set; }

    //case create other user
    public StaffProfileData? StaffProfileData { get; set; }
}
