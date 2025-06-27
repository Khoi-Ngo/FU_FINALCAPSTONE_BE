using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using Microsoft.EntityFrameworkCore;
using AISEA.ApiService.SHARED.DTOs.Responses.Message;

namespace AISEA.ApiService.DAL.Repositories;

public class MessageRepository : GenericRepository<Message>
{
    public MessageRepository(AiseaContext context) : base(context)
    {
    }

    public async Task<List<MessageItemResponse>> GetMessagesAsync(long chatSessionId)
    {
        return await _context.Messages
            .Where(m => m.AdvisorySession1to1Id == chatSessionId)
            .Select(m => new MessageItemResponse
            {
                MessageId = m.Id,
                SenderUsername = m.Sender.Username,
                Content = m.Content,
                SentAt = (DateTime)m.CreatedAt
            })
            .ToListAsync();
    }
}
