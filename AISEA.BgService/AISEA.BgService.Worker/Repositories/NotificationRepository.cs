using AISEA.BgService.Worker.Abstract;
using AISEA.BgService.Worker.Entities;
using AISEA.BgService.Worker.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AISEA.BgService.Worker.Repositories
{
    public class NotificationRepository : GenericRepository<Notification>
    {
        public NotificationRepository(AiseaContext context) : base(context)
        {
        }

        public async Task<List<long>> RemoveAllExistedOverDaysAsync(int expiredDays)
        {
            var thresholdDate = DateTime.UtcNow.AddDays(-expiredDays);

            var expiredNotifications = await _context.Notifications
                .Where(n => n.CreatedAt < thresholdDate)
                .ToListAsync();

            _context.Notifications.RemoveRange(expiredNotifications);
            await _context.SaveChangesAsync();

            return expiredNotifications.Select(n => n.Id).ToList();
        }
    }
}