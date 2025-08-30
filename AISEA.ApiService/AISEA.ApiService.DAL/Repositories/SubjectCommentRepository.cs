using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories
{
    public class SubjectCommentRepository : GenericRepository<SubjectComment>
    {
        public SubjectCommentRepository(AiseaContext context) : base(context)
        {
        }

        public async Task<SubjectComment?> GetByStudentAndSubjectAsync(long studentProfileId, long subjectId)
        {
            return await _context.SubjectComments
                .FirstOrDefaultAsync(c => c.StudentProfileId == studentProfileId
                    && c.SubjectId == subjectId);
        }

        public async Task<SubjectComment?> GetByIdWithDetailsAsync(long id)
        {
            return await _context.SubjectComments
                .Include(c => c.StudentProfile)
                    .ThenInclude(sp => sp.User)
                .Include(c => c.Subject)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<(IEnumerable<SubjectComment> Comments, int TotalCount)> GetPagedBySubjectAsync(
            long subjectId, int pageNumber, int pageSize)
        {
            var query = _context.SubjectComments
                .Include(c => c.StudentProfile)
                    .ThenInclude(sp => sp.User)
                .Where(c => c.SubjectId == subjectId);

            var totalCount = await query.CountAsync();
            var comments = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (comments, totalCount);
        }



        public async Task<List<SubjectComment>> GetCommentsByStudentAsync(long studentProfileId)
        {
            return await _context.SubjectComments
                .Include(c => c.Subject)
                .Where(c => c.StudentProfileId == studentProfileId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }


    }
}
