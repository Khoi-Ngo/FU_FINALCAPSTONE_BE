namespace AISEA.ApiService.SHARED.Interfaces;

public interface IGitRepoService
{
    Task<object> GetRepoDataAsync(string owner, string repoName);
    Task<object> FilterAndAggregateToFocusMainDataOfRepo(string owner, string repoName);
}