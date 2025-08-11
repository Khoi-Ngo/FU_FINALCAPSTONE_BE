using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using AISEA.ApiService.SHARED.DTOs.Responses.AuditLog;
using AISEA.ApiService.SHARED.DTOs.Responses.Pagin;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories;

public class AuditLogRepository : GenericRepository<AuditLog>
{
    public AuditLogRepository(AiseaContext context) : base(context)
    {
    }

    public async Task<PagedResult<AuditLogDTO>> GetPagedAsync(int pageNumber, int pageSize)
    {
        var query = _context.AuditLogs.OrderByDescending(a => a.CreatedAt);
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtoItems = items.Select(a => new AuditLogDTO
        {
            Id = a.Id,
            Tag = a.Tag.ToString(),
            CreatedAt = a.CreatedAt,
            Description = a.Description,
            IsSuccessAction = a.IsSuccessAction
        }).ToList();

        return new PagedResult<AuditLogDTO>
        {
            Items = dtoItems,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<Dictionary<string, Dictionary<string, List<AuditLogDTO>>>> GetCountGroupedByMonthAndYearAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.AuditLogs.AsQueryable();

        if (startDate.HasValue)
            query = query.Where(a => a.CreatedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(a => a.CreatedAt <= endDate.Value);

        var logs = await query.ToListAsync();

        var dict = logs
            .GroupBy(a => new { a.CreatedAt.Year, a.CreatedAt.Month })
            .ToDictionary(
                g => $"{g.Key.Month:D2}/{g.Key.Year}",
                g => g.GroupBy(a => a.Tag.ToString())
                      .ToDictionary(
                          tg => tg.Key,
                          tg => tg.Select(a => new AuditLogDTO
                          {
                              Id = a.Id,
                              Tag = a.Tag.ToString(),
                              CreatedAt = a.CreatedAt,
                              Description = a.Description,
                              IsSuccessAction = a.IsSuccessAction
                          }).ToList()
                      )
            );

        return dict;
    }

}