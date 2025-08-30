using AISEA.ApiService.BAL.Services.CourseTracker;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.Filters;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using AISEA.ApiService.WebApi.InterceptorAPI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.CourseTracker
{
    [ApiController]
    [Route("api/[controller]")]
    public class GitRepoController : BaseController
    {
        private readonly GitRepoService _gitRepoService;
        public GitRepoController(EndpointSettings endpointSettings
        , GitRepoService gitRepoService) : base(endpointSettings)
        {
            _gitRepoService = gitRepoService;
        }



        ///<summary>
        /// Update the PUBLIC github repo to ONE JOINED SUBJECT
        /// </summary>
        [HttpPut("{joinedSubjectId}")]
        [PermissionAuthorize((int)EUserRole.STUDENT)]
        [AuditLog(Tag = "UPDATE_PUBLIC_GITHUB_REPO_URL_JOINED_SUBJECT")]
        public async Task<IActionResult> UpdateRepoURL([FromQuery] string publicRepoURL, long joinedSubjectId)
        {
            await _gitRepoService.UpdateGitRepoURLAsync(joinedSubjectId, publicRepoURL, AccessToken);
            return Ok("Update successfully");
        }



        ///<summary>
        /// View data
        /// </summary>
        [HttpGet]
        [PermissionAuthorize((int)EUserRole.STUDENT)]
        [AuditLog(Tag = "VIEW_METRICS_GIT_REPO_JOINED_SUBJECT")]
        public async Task<IActionResult> View([FromQuery] string owner, [FromQuery] string repoName)
        {
            var res = await _gitRepoService.ViewGitRepoAsync(owner, repoName);
            return Ok(res);
        }

    }
}