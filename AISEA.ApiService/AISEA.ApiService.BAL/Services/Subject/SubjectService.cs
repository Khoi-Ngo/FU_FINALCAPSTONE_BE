using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.DTOs.Requests.Subject;
using AISEA.ApiService.SHARED.DTOs.Responses.Pagin;
using AISEA.ApiService.SHARED.DTOs.Responses.Subject;
using AISEA.ApiService.SHARED.Exceptions;
using AutoMapper;

namespace AISEA.ApiService.BAL.Services.Subject
{
    public class SubjectService
    {
        private readonly SubjectRepository _subjectRepository;
        private readonly SubjectPrerequisiteRepository _prerequisiteRepository;
        private readonly IMapper _mapper;

        public SubjectService(SubjectRepository subjectRepository, SubjectPrerequisiteRepository prerequisiteRepository, IMapper mapper)
        {
            _subjectRepository = subjectRepository;
            _prerequisiteRepository = prerequisiteRepository;
            _mapper = mapper;
        }

        public async Task CreateSubjectAsync(CreateSubjectRequest request)
        {
            var existingSubject = await _subjectRepository.GetByCodeAsync(request.SubjectCode);
            if (existingSubject != null)
            {
                throw new InvalidUserCreatedException($"Subject with code '{request.SubjectCode}' already exists.");
            }

            var subject = _mapper.Map<DAL.Entities.Subject>(request);
            subject.CreatedAt = DateTime.UtcNow;
            
            await _subjectRepository.CreateAsync(subject);
        }

        public async Task<PagedResult<GetSubjectResponse>> GetSubjectsPagedAsync(PaginationRequest request, string? search = null)
        {
            var (subjects, totalCount) = await _subjectRepository.GetPagedAsync(request.PageNumber, request.PageSize, search);
            
            return new PagedResult<GetSubjectResponse>
            {
                Items = _mapper.Map<List<GetSubjectResponse>>(subjects),
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<GetSubjectResponse> GetSubjectByIdAsync(long id)
        {
            var subject = await _subjectRepository.GetByIdAsync(id);
            if (subject == null || subject.IsDeleted)
            {
                throw new NotFoundException("Subject not found.");
            }

            return _mapper.Map<GetSubjectResponse>(subject);
        }

        public async Task UpdateSubjectAsync(long id, UpdateSubjectRequest request)
        {
            var subject = await _subjectRepository.GetByIdAsync(id);
            if (subject == null || subject.IsDeleted)
            {
                throw new NotFoundException("Subject not found.");
            }

            // Check if subject code is being changed and if it conflicts with existing
            if (subject.SubjectCode != request.SubjectCode)
            {
                var existingSubject = await _subjectRepository.GetByCodeAsync(request.SubjectCode);
                if (existingSubject != null && existingSubject.Id != id)
                {
                    throw new InvalidUserCreatedException($"Subject with code '{request.SubjectCode}' already exists.");
                }
            }

            _mapper.Map(request, subject);
            subject.UpdatedAt = DateTime.UtcNow;
            
            await _subjectRepository.UpdateAsync(subject);
        }

        public async Task DeleteSubjectAsync(long id)
        {
            var subject = await _subjectRepository.GetByIdAsync(id);
            if (subject == null || subject.IsDeleted)
            {
                throw new NotFoundException("Subject not found.");
            }

            subject.IsDeleted = true;
            subject.DeletedAt = DateTime.UtcNow;
            
            await _subjectRepository.UpdateAsync(subject);
        }

        public async Task AddPrerequisiteAsync(long subjectId, long prerequisiteSubjectId)
        {
            var subject = await _subjectRepository.GetByIdAsync(subjectId);
            if (subject == null || subject.IsDeleted)
            {
                throw new NotFoundException("Subject not found.");
            }

            var prerequisiteSubject = await _subjectRepository.GetByIdAsync(prerequisiteSubjectId);
            if (prerequisiteSubject == null || prerequisiteSubject.IsDeleted)
            {
                throw new NotFoundException("Prerequisite subject not found.");
            }

            if (subjectId == prerequisiteSubjectId)
            {
                throw new InvalidUserCreatedException("A subject cannot be a prerequisite of itself.");
            }

            var hasPrerequisite = await _prerequisiteRepository.ExistsAsync(subjectId, prerequisiteSubjectId);
            if (hasPrerequisite)
            {
                throw new InvalidUserCreatedException("This prerequisite relationship already exists.");
            }

            var prerequisite = new DAL.Entities.SubjectPrerequisite
            {
                SubjectId = subjectId,
                PrerequisiteSubjectId = prerequisiteSubjectId,
                CreatedAt = DateTime.UtcNow
            };

            await _prerequisiteRepository.CreateAsync(prerequisite);
        }

        public async Task<List<GetSubjectResponse>> GetPrerequisitesAsync(long subjectId)
        {
            var subject = await _subjectRepository.GetByIdAsync(subjectId);
            if (subject == null || subject.IsDeleted)
            {
                throw new NotFoundException("Subject not found.");
            }

            var prerequisites = await _prerequisiteRepository.GetPrerequisitesBySubjectIdAsync(subjectId);
            return _mapper.Map<List<GetSubjectResponse>>(prerequisites);
        }

        public async Task RemovePrerequisiteAsync(long subjectId, long prerequisiteSubjectId)
        {
            var subject = await _subjectRepository.GetByIdAsync(subjectId);
            if (subject == null || subject.IsDeleted)
            {
                throw new NotFoundException("Subject not found.");
            }

            await _prerequisiteRepository.RemovePrerequisiteAsync(subjectId, prerequisiteSubjectId);
        }
        public async Task<bool> CreateSubjectsAsync(List<CreateSubjectRequest> requests)
        {
            foreach (var request in requests)
            {
                var existingSubject = await _subjectRepository.GetByCodeAsync(request.SubjectCode);
                if (existingSubject != null)
                {
                    throw new InvalidUserCreatedException($"Subject with code '{request.SubjectCode}' already exists.");
                }

                var subject = _mapper.Map<DAL.Entities.Subject>(request);
                subject.CreatedAt = DateTime.UtcNow;

                await _subjectRepository.CreateAsync(subject);
            }

            return true;
        }
    }
}