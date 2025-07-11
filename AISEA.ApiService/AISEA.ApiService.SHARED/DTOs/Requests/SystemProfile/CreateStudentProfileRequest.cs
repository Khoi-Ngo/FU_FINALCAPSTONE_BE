namespace AISEA.ApiService.SHARED.DTOs.Requests.SystemProfile
{
    public class CreateStudentProfileRequest
    {
        public long UserId { get; set; }
        public DateTimeOffset EnrolledAt { get; set; }
        public string? CareerGoal { get; set; }
    }
}