using AISEA.ApiService.BAL.Services.CourseTracker;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.Filters;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using AISEA.ApiService.WebApi.InterceptorAPI;
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


        ///<summary>
        /// View Current Self git account username
        /// </summary>
        [HttpGet("git-acc-username")]
        [PermissionAuthorize((int)EUserRole.STUDENT)]
        [AuditLog(Tag = "VIEW_GIT_ACCOUNT_USERNAME")]
        public async Task<IActionResult> ViewCurGitUsername()
        {
            var res = await _gitRepoService.ViewCurGitUsernameASync(AccessToken);
            return Ok(res);
        }

        ///<summary>
        /// Self update git account username
        /// </summary>
        [HttpPut("git-acc-username")]
        [PermissionAuthorize((int)EUserRole.STUDENT)]
        [AuditLog(Tag = "UPDATE_GIT_ACCOUNT_USERNAME")]
        public async Task<IActionResult> UpdateGitUsername([FromQuery] string updatedGitUsername)
        {
            await _gitRepoService.UpdateGitUsernameAsync(AccessToken, updatedGitUsername);
            return Ok("Update successfully");
        }

        ///<summary>
        /// View metrics data of 
        /// </summary>
        [HttpGet("git-acc/metrics")]
        [PermissionAuthorize((int)EUserRole.STUDENT)]
        [AuditLog(Tag = "VIEW_DATA_METRIC_GIT_ACCOUNT")]
        public async Task<IActionResult> ViewDataMetricOfGitUsername([FromQuery] string gitAccountUsername)
        {
            var res = await _gitRepoService.ViewDataMetricOfGitUsernameAsync(gitAccountUsername);
            return Ok(res);
        }



    }
}