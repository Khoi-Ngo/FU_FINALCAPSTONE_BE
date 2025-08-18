namespace AISEA.ApiService.SHARED.DTOs.Responses.CheckPoint;

public class CheckpointListItemResponse
{
    public long Id { get; set; }
    public string Title { get; set; }
    public bool IsCompleted { get; set; } = false;
    public DateTime Deadline { get; set; }
}