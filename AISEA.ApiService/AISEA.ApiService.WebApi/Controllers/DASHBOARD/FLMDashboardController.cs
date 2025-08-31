using Microsoft.AspNetCore.Mvc;
using AISEA.ApiService.BAL.Services.Dashboard;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.Filters;
using AISEA.ApiService.WebApi.Base;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.InterceptorAPI;

namespace AISEA.ApiService.WebApi.Controllers.DASHBOARD
{
    [ApiController]
    [Route("api/[controller]")]
    public class FLMDashboardController : BaseController
    {
        private readonly FLMDashboardService _dashboardService;

        public FLMDashboardController(
            EndpointSettings endpointSettings,
            FLMDashboardService dashboardService) : base(endpointSettings)
        {
            _dashboardService = dashboardService;
        }

        /// <summary>
        /// Get FLM Dashboard overview with general statistics
        /// </summary>
        /// <returns>Overview statistics including total counts and approval status distribution</returns>
        [HttpGet("overview")]
        [PermissionAuthorize((int)EUserRole.ACADEMIC_STAFF, (int)EUserRole.MANAGER, (int)EUserRole.ADMIN)]
        [AuditLog(Tag = "VIEW_FLM_DASHBOARD_OVERVIEW")]
        public async Task<IActionResult> GetOverview()
        {
            var result = await _dashboardService.GetOverviewAsync();
            return Ok(result);
        }

        /// <summary>
        /// Get detailed subject statistics for FLM Dashboard
        /// </summary>
        /// <returns>Subject statistics including distribution by program, credits, and syllabus availability</returns>
        [HttpGet("subjects/statistics")]
        [PermissionAuthorize((int)EUserRole.ACADEMIC_STAFF, (int)EUserRole.MANAGER, (int)EUserRole.ADMIN)]
        [AuditLog(Tag = "VIEW_FLM_SUBJECT_STATISTICS")]
        public async Task<IActionResult> GetSubjectStatistics()
        {
            var result = await _dashboardService.GetSubjectStatisticsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Get detailed curricula statistics for FLM Dashboard
        /// </summary>
        /// <returns>Curricula statistics including distribution by program and semester completeness</returns>
        [HttpGet("curricula/statistics")]
        [PermissionAuthorize((int)EUserRole.ACADEMIC_STAFF, (int)EUserRole.MANAGER, (int)EUserRole.ADMIN)]
        [AuditLog(Tag = "VIEW_FLM_CURRICULA_STATISTICS")]
        public async Task<IActionResult> GetCurriculaStatistics()
        {
            var result = await _dashboardService.GetCurriculaStatisticsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Get recent activities and pending items for FLM Dashboard
        /// </summary>
        /// <returns>Recent activities including new subjects, approved syllabi, and pending items</returns>
        [HttpGet("subjects/recent-activities")]
        [PermissionAuthorize((int)EUserRole.ACADEMIC_STAFF, (int)EUserRole.MANAGER, (int)EUserRole.ADMIN)]
        [AuditLog(Tag = "VIEW_FLM_RECENT_ACTIVITIES")]
        public async Task<IActionResult> GetRecentActivities()
        {
            var result = await _dashboardService.GetRecentActivitiesAsync();
            return Ok(result);
        }

        /// <summary>
        /// Clear FLM Dashboard cache (Admin only)
        /// </summary>
        /// <returns>Success message</returns>
        [HttpPost("cache/clear")]
        [PermissionAuthorize((int)EUserRole.ADMIN)]
        [AuditLog(Tag = "CLEAR_FLM_DASHBOARD_CACHE")]
        public async Task<IActionResult> ClearCache()
        {
            await _dashboardService.ClearCacheAsync();
            return Ok(new { Message = "FLM Dashboard cache cleared successfully" });
        }
    }
}