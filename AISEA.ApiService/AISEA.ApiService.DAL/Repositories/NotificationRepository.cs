using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using AISEA.ApiService.SHARED.DTOs.Responses.Noti;
using AISEA.ApiService.SHARED.DTOs.Responses.Pagin;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories;

public class NotificationRepository : GenericRepository<Notification>
{
    public NotificationRepository(AiseaContext context) : base(context)
    {
    }

    public async Task<PagedResult<NotificationItemResponse>> GetNotificationsAsync(long userId, int pageNumber, int pageSize)
    {
        var query = _context.Notifications
            .Where(n => n.UserId == userId)
            .Select(n => new NotificationItemResponse
            {
                Id = n.Id,
                Title = n.Title,
                Content = n.Content,
                Link = n.Link,
                CreatedAt = n.CreatedAt,
                IsRead = n.IsRead
            });

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<NotificationItemResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task DeleteByIdASync(long notificationId)
    {
        var notification = await _context.Notifications.FindAsync(notificationId);
        if (notification != null)
        {
            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<long>> RemoveAllExistedOverDaysAsync(int expiredDays)
    {
        var thresholdDate = DateTime.UtcNow.AddDays(-expiredDays);

        var expiredNotifications = await _context.Notifications
            .Where(n => n.CreatedAt < thresholdDate && n.IsRead == true)
            .ToListAsync();

        _context.Notifications.RemoveRange(expiredNotifications);
        await _context.SaveChangesAsync();

        return expiredNotifications.Select(n => n.Id).ToList();
    }
}