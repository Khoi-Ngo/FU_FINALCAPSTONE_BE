using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.DTOs.Responses.AuditLog;
using AISEA.ApiService.SHARED.DTOs.Responses.Pagin;

namespace AISEA.ApiService.BAL.Services.AuditLog
{
    public class AuditLogService
    {
        private readonly AuditLogRepository _auditLogRepository;
        public AuditLogService(AuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        /// <summary>
        /// Retrieves all audit log entries.
        /// </summary>
        public async Task<PagedResult<AuditLogDTO>> GetPagedAsync(PaginationRequest request)
        {
            return await _auditLogRepository.GetPagedAsync(request.PageNumber, request.PageSize);
        }

        /// <summary>
        /// Retrieves analytics data for audit logs, including time series, tag distribution, and user activity.
        /// </summary>
        public async Task<AuditLogAnalyticsDTO> GetAnalyticsAsync(DateTime? startDate, DateTime? endDate, string interval)
        {
            return await _auditLogRepository.GetAnalyticsAsync(startDate, endDate, interval);
        }

        public async Task CreateAsync(DAL.Entities.AuditLog auditLog)
        {
            await _auditLogRepository.CreateAsync(auditLog);
        }
    }
}