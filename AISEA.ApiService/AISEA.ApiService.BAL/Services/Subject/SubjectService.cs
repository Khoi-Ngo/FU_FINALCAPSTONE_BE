using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.Const.Values;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.DTOs.Requests.Subject;
using AISEA.ApiService.SHARED.DTOs.Responses.Pagin;
using AISEA.ApiService.SHARED.DTOs.Responses.Subject;
using AISEA.ApiService.SHARED.Exceptions;
using AISEA.ApiService.SHARED.Interfaces;
using AutoMapper;
using Newtonsoft.Json;

namespace AISEA.ApiService.BAL.Services.Subject
{
    public class SubjectService
    {
        private readonly SubjectRepository _subjectRepository;
        private readonly IJWTService _jwtService;
        private readonly IMapper _mapper;
        private readonly IChatOpenAIService _chatOpenAIService;

        public SubjectService(SubjectRepository subjectRepository, IJWTService jwtService, IMapper mapper, IChatOpenAIService chatOpenAIService)
        {
            _subjectRepository = subjectRepository;
            _jwtService = jwtService;
            _mapper = mapper;
            _chatOpenAIService = chatOpenAIService;
        }

        public async Task CreateSubjectAsync(CreateSubjectRequest request, string accessToken)
        {
            var existingSubject = await _subjectRepository.GetByCodeAsync(request.SubjectCode);
            if (existingSubject != null)
            {
                throw new InvalidUserCreatedException($"Subject with code '{request.SubjectCode}' already exists.");
            }

            var createdBy = _jwtService.GetUsernameFromToken(accessToken);
            var subject = _mapper.Map<DAL.Entities.Subject>(request);
            subject.CreatedBy = createdBy;
            subject.CreatedAt = DateTime.UtcNow;

            await _subjectRepository.CreateAsync(subject);
        }

        public async Task<PagedResult<GetSubjectResponse>> GetSubjectsPagedAsync(PaginationRequest request, string? search = null, string? comboName = null, string? curriculumCode = null)
        {
            var (subjects, totalCount) = await _subjectRepository.GetPagedAsync(request.PageNumber, request.PageSize, search, comboName, curriculumCode);

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

        public async Task<bool> CreateSubjectsAsync(List<CreateSubjectRequest> requests, string accessToken)
        {
            var createdBy = _jwtService.GetUsernameFromToken(accessToken);

            foreach (var request in requests)
            {
                var existingSubject = await _subjectRepository.GetByCodeAsync(request.SubjectCode);
                if (existingSubject != null)
                {
                    // Skip existing subjects and continue with the next one
                    continue;
                }

                var subject = _mapper.Map<DAL.Entities.Subject>(request);
                subject.CreatedBy = createdBy;
                subject.CreatedAt = DateTime.UtcNow;

                await _subjectRepository.CreateAsync(subject);
            }

            return true;
        }


        public async Task<string> GenTempTipForSubjectAsync(long id)
        {
            var subject = await _subjectRepository.GetByIdWithAllRelatedAsync(id);
            if (subject == null) return "Subject not found.";

            var subjectJson = JsonConvert.SerializeObject(
                subject,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

            var prompt = string.Format(CallAIConst.TemplatePromptFroTempGenTipForASubject, subjectJson);
            return await _chatOpenAIService.SendMsgAsync(prompt);
        }

    }
}