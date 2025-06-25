using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using AISEA.ApiService.SHARED.PropConfigs;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories
{
    public class AdvisorySession1to1Repository : GenericRepository<AdvisorySession1to1>
    {
        private readonly StaffUserSettings _staffUserSettings;
        public AdvisorySession1to1Repository(AiseaContext context, StaffUserSettings staffUserSettings) : base(context)
        {
            _staffUserSettings = staffUserSettings;
        }

        public async Task<(IEnumerable<AdvisorySession1to1> AdvisorySession1To1s, int TotalCount)> GetAllByStaffSelfPagedAsync(int pageNumber, int pageSize, long staffProfileId)
        {
            var query = _context.AdvisorySessions1to1.Where(s => s.StaffId == staffProfileId && s.StaffId != (long)_staffUserSettings.EmptyStaffProfileId);
            var totalCount = await query.CountAsync();
            var sessions = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
            return (sessions, totalCount);
        }

        public async Task<(IEnumerable<AdvisorySession1to1> AdvisorySession1To1s, int TotalCount)> GetAllOpenPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.AdvisorySessions1to1.Where(s => s.StaffId == (long)_staffUserSettings.EmptyStaffProfileId);
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

        public async Task<AdvisorySession1to1> GetWMessagesByIdAsync(long id, long studentProfileId)
        {
            return await _context.AdvisorySessions1to1
                .Include(s => s.Messages)
                    .ThenInclude(m => m.Sender)
                .FirstOrDefaultAsync(s => s.Id == id &&
                 s.StudentId == studentProfileId
                );
        }

        public async Task<AdvisorySession1to1> GetByIdAsync(long id, long profileId)
        {
            return await _context.AdvisorySessions1to1
                    .FirstOrDefaultAsync(s => s.Id == id &&
                    (s.StaffId == profileId || s.StudentId == profileId));
        }
        public async Task<AdvisorySession1to1> GetUnknownStatByIdAsync(long id, long profileId)
        {
            return await _context.AdvisorySessions1to1
                    .FirstOrDefaultAsync(s => s.Id == id &&
                    (s.StaffId == profileId || s.StudentId == profileId || s.StaffId == (long)_staffUserSettings.EmptyStaffProfileId));
        }

    }
}