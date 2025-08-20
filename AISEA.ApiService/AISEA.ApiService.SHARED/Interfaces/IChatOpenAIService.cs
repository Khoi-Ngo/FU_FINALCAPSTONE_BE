using AISEA.ApiService.SHARED.DTOs.Requests.CheckPoint;

namespace AISEA.ApiService.SHARED.Interfaces;

public interface IChatOpenAIService
{
    Task<string> SendMsgAsync(string prompt);
    Task<List<CommandCheckpointRequest>> GenerateCheckpoints(string userPrompt);
}