using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories
{
    public class ComboRepository : GenericRepository<Combo>
    {
        public ComboRepository(AiseaContext context) : base(context)
        {
        }

        public async Task<(IEnumerable<Combo> Combos, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? search = null,
            long? programId = null,
            int? semesterNumber = null,
            string? difficultyLevel = null,
            bool? isAvailable = null,
            string? sortBy = "ComboName",
            string? sortOrder = "asc")
        {
            var query = _context.Combos
                .Include(c => c.ComboSubjects)
                    .ThenInclude(cs => cs.Subject)
                .Where(c => !c.IsDeleted);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.ComboName.Contains(search) || 
                                       (c.ComboDescription != null && c.ComboDescription.Contains(search)));
            }

            if (programId.HasValue)
            {
                query = query.Where(c => c.ProgramId == programId.Value);
            }

            if (semesterNumber.HasValue)
            {
                query = query.Where(c => c.SemesterNumber == semesterNumber.Value);
            }

            if (!string.IsNullOrEmpty(difficultyLevel))
            {
                query = query.Where(c => c.DifficultyLevel == difficultyLevel);
            }

            if (isAvailable.HasValue && isAvailable.Value)
            {
                query = query.Where(c => c.CurrentEnrollment < c.MaxStudents);
            }

            // Apply sorting
            query = sortBy?.ToLower() switch
            {
                "semesternumber" => sortOrder?.ToLower() == "desc"
                    ? query.OrderByDescending(c => c.SemesterNumber)
                    : query.OrderBy(c => c.SemesterNumber),
                "difficultylevel" => sortOrder?.ToLower() == "desc"
                    ? query.OrderByDescending(c => c.DifficultyLevel)
                    : query.OrderBy(c => c.DifficultyLevel),
                "maxstudents" => sortOrder?.ToLower() == "desc"
                    ? query.OrderByDescending(c => c.MaxStudents)
                    : query.OrderBy(c => c.MaxStudents),
                _ => sortOrder?.ToLower() == "desc"
                    ? query.OrderByDescending(c => c.ComboName)
                    : query.OrderBy(c => c.ComboName)
            };

            var totalCount = await query.CountAsync();
            var combos = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (combos, totalCount);
        }

        public async Task<Combo?> GetDetailByIdAsync(long id)
        {
            return await _context.Combos
                .Include(c => c.ComboSubjects)
                    .ThenInclude(cs => cs.Subject)
                .Include(c => c.ComboPrerequisites)
                    .ThenInclude(cp => cp.Subject)
                .Include(c => c.StudentEnrollments.Where(se => se.Status == "Active"))
                    .ThenInclude(se => se.Student)
                        .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task AddSubjectAsync(ComboSubject comboSubject)
        {
            _context.ComboSubjects.Add(comboSubject);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveSubjectsByComboIdAsync(long comboId)
        {
            var comboSubjects = await _context.ComboSubjects
                .Where(cs => cs.ComboId == comboId)
                .ToListAsync();
            
            _context.ComboSubjects.RemoveRange(comboSubjects);
            await _context.SaveChangesAsync();
        }
    }
}