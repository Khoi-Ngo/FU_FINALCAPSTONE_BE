namespace AISEA.ApiService.SHARED.DTOs.Responses.AuditLog;

public class AuditLogDTO
{
    public long Id { get; set; }
    public string Tag { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}