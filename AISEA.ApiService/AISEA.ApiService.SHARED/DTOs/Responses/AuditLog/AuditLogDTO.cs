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