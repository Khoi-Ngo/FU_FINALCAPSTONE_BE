using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.Exceptions;
using AISEA.ApiService.SHARED.Interfaces;

namespace AISEA.ApiService.BAL.Services.CourseTracker;

public class GitRepoService
{
    private readonly JoinedSubjectRepository _joinedSubjectRepository;
    private readonly IJWTService _jWTService;
    private readonly IGitRepoService _gitRepoService;

    public GitRepoService(JoinedSubjectRepository joinedSubjectRepository, IJWTService jWTService, IGitRepoService gitRepoService)
    {
        _joinedSubjectRepository = joinedSubjectRepository;
        _jWTService = jWTService;
        _gitRepoService = gitRepoService;
    }

    public async Task<object> ViewGitRepoAsync(string owner, string repoName)
    {

        return await _gitRepoService.GetRepoDataAsync(owner, repoName);
    }

    public async Task UpdateGitRepoURLAsync(long joinedSubjectId, string publicRepoURL, string accessToken)
    {
        var joinedSubject = await _joinedSubjectRepository.GetByIdAsync(joinedSubjectId);
        if (!IsValidAccessJoinedSubject(accessToken, joinedSubject)) throw new InvalidAccessJoinedSubject("You have no permission to access this");
        joinedSubject.GithubRepositoryURL = publicRepoURL;
        await _joinedSubjectRepository.UpdateAsync(joinedSubject);
    }


    private bool IsValidAccessJoinedSubject(string accessToken, JoinedSubject joinedSubject)
    => joinedSubject.StudentProfileId == _jWTService.GetProfileIdFromToken(accessToken);
}