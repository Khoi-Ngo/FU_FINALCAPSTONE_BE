namespace AISEA.ApiService.SHARED.DTOs.Responses.AuditLog;

public class AuditLogDTO
{
    public long Id { get; set; }
    public string Tag { get; set; }
    public string? Description { get; set; }
    public bool IsSuccessAction { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public long? RoleId { get; set; }
    public string? Email { get; set; }
    public string? IPAddress { get; set; }
    public string? UserAgent { get; set; }
    public long? UserId { get; set; }
}

public class AuditLogAnalyticsDTO
{
    public List<TimeSeriesData> TimeSeries { get; set; }
    public Dictionary<string, int> TagDistribution { get; set; }
    public List<UserActivity> TopActiveUsers { get; set; }
    public int TotalLogs { get; set; }
    public double SuccessRate { get; set; }
}

public class TimeSeriesData
{
    public string Period { get; set; }
    public int TotalLogs { get; set; }
    public Dictionary<string, int> LogsByTag { get; set; }
    public double SuccessRate { get; set; }
}

public class UserActivity
{
    public long? UserId { get; set; }
    public string? UserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int LogCount { get; set; }
}