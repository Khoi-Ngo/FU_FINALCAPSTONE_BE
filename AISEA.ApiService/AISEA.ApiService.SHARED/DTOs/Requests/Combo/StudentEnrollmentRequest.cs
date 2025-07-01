namespace AISEA.ApiService.SHARED.DTOs.Requests.Combo
{
    public class StudentEnrollmentRequest
    {
        public long ComboId { get; set; }
        public long StudentId { get; set; }
        public string? Notes { get; set; }
    }

    public class BulkEnrollmentRequest
    {
        public long ComboId { get; set; }
        public List<long> StudentIds { get; set; } = new();
        public string? Notes { get; set; }
    }
}