namespace AISEA.ApiService.SHARED.DTOs.Requests.User;

public class BulkCreateStudentRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTimeOffset? DateOfBirth { get; set; }
    public StudentProfileData? StudentProfileData { get; set; }
}
