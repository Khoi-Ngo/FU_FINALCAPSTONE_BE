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
        /// Retrieves the count as dictionary Dictionary<MonthYear, Dictionary<Tag, Count>> of audit log entries grouped by month and year. (all)
        /// </summary>
        public async Task<Dictionary<string, Dictionary<string, List<AuditLogDTO>>>> GetCountGroupedByMonthAndYearAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            return await _auditLogRepository.GetCountGroupedByMonthAndYearAsync(startDate, endDate);
        }

        public async Task CreateAsync(DAL.Entities.AuditLog auditLog)
        {
            await _auditLogRepository.CreateAsync(auditLog);
        }
    }
}