using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.Approval;
using AISEA.ApiService.SHARED.Exceptions;
using AISEA.ApiService.SHARED.Interfaces;
using AutoMapper;

namespace AISEA.ApiService.BAL.Services.Approval
{
    public class ApprovalService
    {
        private readonly SubjectRepository _subjectRepository;
        private readonly CurriculumRepository _curriculumRepository;
        private readonly SyllabusRepository _syllabusRepository;
        private readonly ComboRepository _comboRepository;
        private readonly IJWTService _jwtService;

        public ApprovalService(
            SubjectRepository subjectRepository,
            CurriculumRepository curriculumRepository,
            SyllabusRepository syllabusRepository,
            ComboRepository comboRepository,
            IJWTService jwtService)
        {
            _subjectRepository = subjectRepository;
            _curriculumRepository = curriculumRepository;
            _syllabusRepository = syllabusRepository;
            _comboRepository = comboRepository;
            _jwtService = jwtService;
        }

        public async Task ApproveOrRejectSubjectAsync(long subjectId, ApprovalRequest request, string accessToken)
        {
            var subject = await _subjectRepository.GetByIdAsync(subjectId);
            if (subject == null || subject.IsDeleted)
            {
                throw new NotFoundException("Subject not found.");
            }

            var approver = _jwtService.GetUsernameFromToken(accessToken);

            subject.ApprovalStatus = request.ApprovalStatus;
            subject.ApprovedBy = approver;
            subject.ApprovedAt = DateTime.UtcNow;
            subject.RejectionReason = request.RejectionReason;

            await _subjectRepository.UpdateAsync(subject);
        }

        public async Task ApproveOrRejectCurriculumAsync(long curriculumId, ApprovalRequest request, string accessToken)
        {
            var curriculum = await _curriculumRepository.GetByIdAsync(curriculumId);
            if (curriculum == null || curriculum.IsDeleted)
            {
                throw new NotFoundException("Curriculum not found.");
            }

            var approver = _jwtService.GetUsernameFromToken(accessToken);

            curriculum.ApprovalStatus = request.ApprovalStatus;
            curriculum.ApprovedBy = approver;
            curriculum.ApprovedAt = DateTime.UtcNow;
            curriculum.RejectionReason = request.RejectionReason;

            await _curriculumRepository.UpdateAsync(curriculum);
        }

        public async Task ApproveOrRejectSyllabusAsync(long syllabusId, ApprovalRequest request, string accessToken)
        {
            var syllabus = await _syllabusRepository.GetByIdAsync(syllabusId);
            if (syllabus == null || syllabus.IsDeleted)
            {
                throw new NotFoundException("Syllabus not found.");
            }

            var approver = _jwtService.GetUsernameFromToken(accessToken);

            syllabus.ApprovalStatus = request.ApprovalStatus;
            syllabus.ApprovedBy = approver;
            syllabus.ApprovedAt = DateTime.UtcNow;
            syllabus.RejectionReason = request.RejectionReason;

            await _syllabusRepository.UpdateAsync(syllabus);
        }

        public async Task ApproveOrRejectComboAsync(long comboId, ApprovalRequest request, string accessToken)
        {
            var combo = await _comboRepository.GetByIdAsync(comboId);
            if (combo == null || combo.IsDeleted)
            {
                throw new NotFoundException("Combo not found.");
            }

            var approver = _jwtService.GetUsernameFromToken(accessToken);

            combo.ApprovalStatus = request.ApprovalStatus;
            combo.ApprovedBy = approver;
            combo.ApprovedAt = DateTime.UtcNow;
            combo.RejectionReason = request.RejectionReason;

            await _comboRepository.UpdateAsync(combo);
        }
    }
}
