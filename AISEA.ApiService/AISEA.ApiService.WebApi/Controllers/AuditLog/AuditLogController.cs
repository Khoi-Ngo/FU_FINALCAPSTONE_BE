using AISEA.ApiService.BAL.Services.AuditLog;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.Filters;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using AISEA.ApiService.WebApi.InterceptorAPI;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.AuditLog
{
    [ApiController]
    [Route("api/[controller]")]
    [PermissionAuthorize((int)EUserRole.ADMIN)]
    public class AuditLogController : BaseController
    {
        private readonly AuditLogService _auditLogService;
        public AuditLogController(EndpointSettings endpointSettings, AuditLogService auditLogService) : base(endpointSettings)
        {
            _auditLogService = auditLogService;
        }

        /// <summary>
        /// Get paged audit logs
        /// </summary>
        [HttpGet]
        [AuditLog(Tag = "VIEW_AUDIT_LOGS")]
        public async Task<IActionResult> GetAuditLogs([FromQuery] PaginationRequest request)
        {
            var result = await _auditLogService.GetPagedAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Get Dictionary of audit logs (Dictionary<YearMonth, Dictionary<EAuditLogTag, List<AuditLog>>>)
        /// </summary>
        [HttpGet("all")]
        [AuditLog(Tag = "VIEW_AUDIT_LOGS")]
        public async Task<IActionResult> GetAllAuditLogs([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var result = await _auditLogService.GetCountGroupedByMonthAndYearAsync(startDate, endDate);
            return Ok(result);
        }

        /// <summary>
        /// Get analytics data for audit logs (for charts and tables)
        /// </summary>
        [HttpGet("analytics")]
        [AuditLog(Tag = "VIEW_AUDIT_LOG_ANALYTICS")]
        public async Task<IActionResult> GetAuditLogAnalytics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] string interval = "daily")
        {
            var result = await _auditLogService.GetAnalyticsAsync(startDate, endDate, interval);
            return Ok(result);
        }
    }
}