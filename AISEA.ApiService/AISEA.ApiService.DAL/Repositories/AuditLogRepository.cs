using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using AISEA.ApiService.SHARED.DTOs.Responses.AuditLog;
using AISEA.ApiService.SHARED.DTOs.Responses.Pagin;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AISEA.ApiService.DAL.Repositories
{
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
                IsSuccessAction = a.IsSuccessAction,
                UserName = a.UserName,
                FirstName = a.FirstName,
                LastName = a.LastName,
                RoleId = a.RoleId,
                Email = a.Email,
                IPAddress = a.IPAddress,
                UserAgent = a.UserAgent,
                UserId = a.UserId
            }).ToList();

            return new PagedResult<AuditLogDTO>
            {
                Items = dtoItems,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }


        public async Task<AuditLogAnalyticsDTO> GetAnalyticsAsync(DateTime? startDate, DateTime? endDate, string interval)
        {
            var query = _context.AuditLogs.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(a => a.CreatedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(a => a.CreatedAt <= endDate.Value);

            var logs = await query.ToListAsync();

            // Determine grouping based on interval
            Func<AuditLog, string> timeGrouper;
            switch (interval.ToLower())
            {
                case "weekly":
                    timeGrouper = a => $"{a.CreatedAt.Year}-W{GetIso8601WeekOfYear(a.CreatedAt)}";
                    break;
                case "monthly":
                    timeGrouper = a => $"{a.CreatedAt.Month:D2}/{a.CreatedAt.Year}";
                    break;
                case "daily":
                default:
                    timeGrouper = a => a.CreatedAt.ToString("yyyy-MM-dd");
                    break;
            }

            // Time series data
            var timeSeries = logs
                .GroupBy(timeGrouper)
                .OrderBy(g => g.Key)
                .Select(g => new TimeSeriesData
                {
                    Period = g.Key,
                    TotalLogs = g.Count(),
                    LogsByTag = g.GroupBy(a => a.Tag)
                                 .ToDictionary(tg => tg.Key, tg => tg.Count()),
                    SuccessRate = g.Any() ? (double)g.Count(a => a.IsSuccessAction) / g.Count() * 100 : 0
                })
                .ToList();

            // Tag distribution
            var tagDistribution = logs
                .GroupBy(a => a.Tag)
                .ToDictionary(g => g.Key, g => g.Count());

            // Top active users
            var topUsers = logs
                .GroupBy(a => new { a.UserId, a.UserName, a.FirstName, a.LastName })
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => new UserActivity
                {
                    UserId = g.Key.UserId,
                    UserName = g.Key.UserName,
                    FirstName = g.Key.FirstName,
                    LastName = g.Key.LastName,
                    LogCount = g.Count()
                })
                .ToList();

            return new AuditLogAnalyticsDTO
            {
                TimeSeries = timeSeries,
                TagDistribution = tagDistribution,
                TopActiveUsers = topUsers,
                TotalLogs = logs.Count,
                SuccessRate = logs.Any() ? (double)logs.Count(a => a.IsSuccessAction) / logs.Count * 100 : 0
            };
        }

        // Helper method to calculate ISO 8601 week number
        private static int GetIso8601WeekOfYear(DateTime date)
        {
            var day = (int)date.DayOfWeek;
            if (day == 0) day = 7; // Convert Sunday to 7
            day--; // Make Monday 0
            var jan1 = new DateTime(date.Year, 1, 1);
            var daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;
            if (daysOffset < 0) daysOffset += 7;
            var firstThursday = jan1.AddDays(daysOffset);
            var firstWeek = GetIso8601WeekOfYear(firstThursday);
            var weekNum = ((date - firstThursday).Days + 10) / 7;
            if (weekNum < 1)
                weekNum = GetIso8601WeekOfYear(date.AddDays(-7));
            else if (weekNum > 52)
            {
                var nextJan1 = new DateTime(date.Year + 1, 1, 1);
                if (nextJan1.DayOfWeek <= DayOfWeek.Wednesday)
                    weekNum = 1;
            }
            return weekNum;
        }
    }
}