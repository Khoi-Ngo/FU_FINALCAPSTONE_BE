using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using Microsoft.EntityFrameworkCore;
using AISEA.ApiService.SHARED.DTOs.Responses.Message;
using AISEA.ApiService.SHARED.DTOs.Responses.Pagin;

namespace AISEA.ApiService.DAL.Repositories;

public class MessageRepository : GenericRepository<Message>
{
    public MessageRepository(AiseaContext context) : base(context)
    {
    }
    public async Task<PagedResult<MessageItemResponse>> GetMessagesAsync(long chatSessionId, int pageNumber, int pageSize)
    {
        var query = _context.Messages
            .Where(m => m.AdvisorySession1to1Id == chatSessionId)
            .Select(m => new MessageItemResponse
            {
                MessageId = m.Id,
                SenderId = m.SenderId,
                AdvisorySession1to1Id = m.AdvisorySession1to1Id,
                Content = m.Content,
                CreatedAt = m.CreatedAt
            });

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(m => m.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<MessageItemResponse>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}
