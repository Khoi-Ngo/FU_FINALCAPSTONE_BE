using AISEA.ApiService.SHARED.DTOs.Requests.CheckPoint;
using AISEA.ApiService.SHARED.DTOs.Responses.SubjectComment;
using AISEA.ApiService.SHARED.DTOs.Roadmap;

namespace AISEA.ApiService.SHARED.Interfaces;

public interface IChatOpenAIService
{
    Task<string> SendMsgAsync(string prompt);
    Task<List<CommandCheckpointRequest>> GenerateCheckpoints(string userPrompt);
    Task<(bool isValid, string? reason)> ValidateCommentAsync(string content);
    Task<CommentVerificationResult> VerifyCommentAsync(string content);
    Task<List<CreateNodeDto>> GenExternalSubjectNodesInStudyRoadmap(string prompt);
    Task<string> GetSuggestedComboForStudent(string prompt);

    Task<List<RoadmapLinkDto>> GetTheCompleteLinkedNodes(string prompt);



    
}
