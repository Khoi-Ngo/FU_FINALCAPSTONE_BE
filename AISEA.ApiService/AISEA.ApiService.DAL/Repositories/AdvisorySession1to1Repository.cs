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
    public class AdvisorySession1to1Repository : GenericRepository<AdvisorySession1to1>
    {
        public AdvisorySession1to1Repository(AiseaContext context) : base(context)
        {
        }

        public async Task<(IEnumerable<AdvisorySession1to1> AdvisorySession1To1s, int TotalCount)> GetAllByStaffSelfPagedAsync(int pageNumber, int pageSize, long staffProfileId)
        {
            var query = _context.AdvisorySessions1to1.Where(s => s.StaffId == staffProfileId);
            var totalCount = await query.CountAsync();
            var sessions = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
            return (sessions, totalCount);
        }

        public async Task<(IEnumerable<AdvisorySession1to1> AdvisorySession1To1s, int TotalCount)> GetAllOpenPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.AdvisorySessions1to1.Where(s => s.StaffId <= 0);
            var totalCount = await query.CountAsync();
            var sessions = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
            return (sessions, totalCount);
        }

        public async Task<(IEnumerable<AdvisorySession1to1> AdvisorySession1To1s, int TotalCount)> GetAllByStudentSelfPagedAsync(int pageNumber, int pageSize, long studentProfileId)
        {
            var query = _context.AdvisorySessions1to1.Where(s => s.StudentId == studentProfileId);
            var totalCount = await query.CountAsync();
            var sessions = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
            return (sessions, totalCount);
        }

        public async Task<AdvisorySession1to1> GetWMessagesByIdAsync(long id, long profileId)
        {
            return await _context.AdvisorySessions1to1
                .Include(s => s.Messages)
                    .ThenInclude(m => m.Sender)
                .FirstOrDefaultAsync(s => s.Id == id &&
                (s.StaffId == profileId || s.StudentId == profileId)
                );
        }

        public async Task<AdvisorySession1to1> GetByIdAsync(long id, long profileId)
        {
            return await _context.AdvisorySessions1to1
                    .FirstOrDefaultAsync(s => s.Id == id &&
                    (s.StaffId == profileId || s.StudentId == profileId));
        }
    }
}