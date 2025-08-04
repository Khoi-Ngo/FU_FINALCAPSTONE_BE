namespace AISEA.ApiService.BAL.Services.CourseTracker
{
    using AISEA.ApiService.DAL.Entities;
    using AISEA.ApiService.DAL.Repositories;
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
    }
}