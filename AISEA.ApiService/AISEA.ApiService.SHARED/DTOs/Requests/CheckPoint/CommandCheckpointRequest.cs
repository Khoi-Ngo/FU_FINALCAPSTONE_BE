namespace AISEA.ApiService.SHARED.DTOs.Requests.CheckPoint;

public class CommandCheckpointRequest
{

    public string Title { get; set; }
    public string Content { get; set; }
    public string? Note { get; set; }
    public string? Link1 { get; set; }
    public string? Link2 { get; set; }
    public string? Link3 { get; set; }
    public string? Link4 { get; set; }
    public string? Link5 { get; set; }
    public DateTime Deadline { get; set; }

}