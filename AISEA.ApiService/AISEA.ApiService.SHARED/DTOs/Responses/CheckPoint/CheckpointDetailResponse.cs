namespace AISEA.ApiService.SHARED.DTOs.Responses.CheckPoint;

public class CheckpointDetailResponse
{
    public long Id { get; set; }

    public string Title { get; set; }
    public string? Content { get; set; }
    public string? Note { get; set; }
    public bool IsCompleted { get; set; } = false;

    public string? Link1 { get; set; }
    public string? Link2 { get; set; }
    public string? Link3 { get; set; }
    public string? Link4 { get; set; }
    public string? Link5 { get; set; }

    public DateTime Deadline { get; set; }
}