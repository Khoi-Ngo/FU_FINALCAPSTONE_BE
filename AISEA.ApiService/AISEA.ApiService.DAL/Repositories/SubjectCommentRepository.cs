using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using AISEA.ApiService.SHARED.Const.Enums;
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

        public async Task<bool> HasUserCommentedOnSubjectAsync(long studentProfileId, long subjectId)
        {
            return await _context.SubjectComments
                .AnyAsync(c => c.StudentProfileId == studentProfileId
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
            long subjectId, int pageNumber, int pageSize, ECommentSortBy sortBy = ECommentSortBy.Date, ESortDirection sortDirection = ESortDirection.Desc)
        {
            var query = _context.SubjectComments
                .Include(c => c.StudentProfile)
                    .ThenInclude(sp => sp.User)
                .Where(c => c.SubjectId == subjectId);

            var totalCount = await query.CountAsync();

            // Apply sorting based on parameters
            query = sortBy switch
            {
                ECommentSortBy.Date => sortDirection == ESortDirection.Desc
                    ? query.OrderByDescending(c => c.CreatedAt)
                    : query.OrderBy(c => c.CreatedAt),

                ECommentSortBy.LikeCount => sortDirection == ESortDirection.Desc
                    ? query.OrderByDescending(c => c.LikedByStudentIds != null ? c.LikedByStudentIds.Split(',', StringSplitOptions.RemoveEmptyEntries).Length : 0)
                           .ThenByDescending(c => c.CreatedAt) // Secondary sort by date
                    : query.OrderBy(c => c.LikedByStudentIds != null ? c.LikedByStudentIds.Split(',', StringSplitOptions.RemoveEmptyEntries).Length : 0)
                           .ThenByDescending(c => c.CreatedAt), // Secondary sort by date

                _ => query.OrderByDescending(c => c.CreatedAt)
            };

            var comments = await query
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

        public async Task<IEnumerable<SubjectComment>> GetAllToValidateAsync()
        {
            return await _context.SubjectComments
                .Where(c => !c.IsScannedToValidate)
                .OrderBy(c => c.CreatedAt)
                .Take(100)
                .ToListAsync();
        }
        public async Task RemoveRangeAsync(IEnumerable<SubjectComment> comments)
        {
            _context.SubjectComments.RemoveRange(comments);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<SubjectComment> comments)
        {
            _context.SubjectComments.UpdateRange(comments);
            await _context.SaveChangesAsync();
        }


    }
}
