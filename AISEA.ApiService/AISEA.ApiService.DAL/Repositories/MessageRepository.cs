using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories
{
    public class MessageRepository : GenericRepository<Message>
    {
        public MessageRepository(AiseaContext context) : base(context)
        {
        }
        public async Task<List<Message>> GetMessagesAsync(long chatSessionId)
        {
            return await _context.Messages.Include(m => m.Sender).Where(m => m.AdvisorySession1to1Id == chatSessionId).ToListAsync();
        }
    }
}