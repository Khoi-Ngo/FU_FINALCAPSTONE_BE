using AISEA.ApiService.BAL.Services.Approval;
using AISEA.ApiService.SHARED.DTOs.Requests.Approval;
using AISEA.ApiService.SHARED.Filters;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.Approval
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApprovalController : BaseController
    {
        private readonly ApprovalService _approvalService;

        public ApprovalController(EndpointSettings endpointSettings, ApprovalService approvalService) : base(endpointSettings)
        {
            _approvalService = approvalService;
        }

        /// <summary>
        /// Approve or reject a subject (Manager only)
        /// </summary>
        [HttpPut("subject/{subjectId}")]
        public async Task<IActionResult> ApproveOrRejectSubject(long subjectId, [FromBody] ApprovalRequest request)
        {
            await _approvalService.ApproveOrRejectSubjectAsync(subjectId, request, AccessToken);
            return Ok(new { Message = $"Subject {request.ApprovalStatus.ToString().ToLower()} successfully." });
        }

        /// <summary>
        /// Approve or reject a curriculum (Manager only)
        /// </summary>
        [HttpPut("curriculum/{curriculumId}")]
        public async Task<IActionResult> ApproveOrRejectCurriculum(long curriculumId, [FromBody] ApprovalRequest request)
        {
            await _approvalService.ApproveOrRejectCurriculumAsync(curriculumId, request, AccessToken);
            return Ok(new { Message = $"Curriculum {request.ApprovalStatus.ToString().ToLower()} successfully." });
        }

        /// <summary>
        /// Approve or reject a syllabus (Manager only)
        /// </summary>
        [HttpPut("syllabus/{syllabusId}")]
        public async Task<IActionResult> ApproveOrRejectSyllabus(long syllabusId, [FromBody] ApprovalRequest request)
        {
            await _approvalService.ApproveOrRejectSyllabusAsync(syllabusId, request, AccessToken);
            return Ok(new { Message = $"Syllabus {request.ApprovalStatus.ToString().ToLower()} successfully." });
        }

        /// <summary>
        /// Approve or reject a combo (Manager only)
        /// </summary>
        [HttpPut("combo/{comboId}")]
        public async Task<IActionResult> ApproveOrRejectCombo(long comboId, [FromBody] ApprovalRequest request)
        {
            await _approvalService.ApproveOrRejectComboAsync(comboId, request, AccessToken);
            return Ok(new { Message = $"Combo {request.ApprovalStatus.ToString().ToLower()} successfully." });
        }
    }
}
