namespace AISEA.ApiService.BAL.Services.CourseTracker
{
    using AISEA.ApiService.DAL.Entities;
    using AISEA.ApiService.DAL.Repositories;
    using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
    using AISEA.ApiService.SHARED.DTOs.Responses.Pagin;

    public class SemesterReferService
    {
        private readonly SemesterRepository _semesterRepository;
        public SemesterReferService(SemesterRepository semesterRepository)
        {
            _semesterRepository = semesterRepository;
        }

        public async Task<bool> SemesterExistsAsync(string semesterName)
        {
            return await _semesterRepository.SemesterExistsAsync(semesterName);
        }

        public async Task AddSemesterAsync(string semesterName, DateTime createdAt)
        {
            await _semesterRepository.CreateAsync(new Semester
            {
                SemesterName = semesterName,
                CreatedAt = createdAt
            });
        }

        public async Task<PagedResult<Semester>> GetAllAsyncPaged(PaginationRequest request)
        {
            return await _semesterRepository.GetAllAsyncPaged(request);
        }
    }
}