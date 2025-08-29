using AISEA.ApiService.BAL.Services.Subject;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.DTOs.Requests.Subject;
using AISEA.ApiService.SHARED.Filters;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.Subject
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubjectController : BaseController
    {
        private readonly SubjectService _subjectService;

        public SubjectController(EndpointSettings endpointSettings, SubjectService subjectService) : base(endpointSettings)
        {
            _subjectService = subjectService;
        }

        /// <summary>
        /// Creates a new subject (Academic Staff only) - Requires Manager approval
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectRequest request)
        {
            await _subjectService.CreateSubjectAsync(request, AccessToken);
            return Ok(new { Message = "Subject created successfully and pending approval." });
        }

        /// <summary>
        /// Creates multiple subjects in bulk (Academic Staff only) - Requires Manager approval
        /// </summary>
        [HttpPost("bulk")]
        public async Task<IActionResult> CreateSubjects([FromBody] List<CreateSubjectRequest> requests)
        {
            await _subjectService.CreateSubjectsAsync(requests, AccessToken);
            return Ok(new { Message = "Subjects created successfully and pending approval." });
        }

        /// <summary>
        /// Gets paginated list of subjects with optional search and filters
        /// </summary>
        /// <param name="request">Pagination parameters</param>
        /// <param name="search">Search by SubjectCode or SubjectName</param>
        /// <param name="comboName">Filter by Combo Name</param>
        /// <param name="curriculumCode">Filter by Curriculum Code</param>
        [HttpGet]
        public async Task<IActionResult> GetSubjects(
            [FromQuery] PaginationRequest request,
            [FromQuery] string? search = null,
            [FromQuery] string? comboName = null,
            [FromQuery] string? curriculumCode = null)
        {
            var result = await _subjectService.GetSubjectsPagedAsync(request, search, comboName, curriculumCode);
            return Ok(result);
        }

        /// <summary>
        /// Gets a subject by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubjectById(long id)
        {
            var result = await _subjectService.GetSubjectByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing subject (Academic Staff only)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubject(long id, [FromBody] UpdateSubjectRequest request)
        {
            await _subjectService.UpdateSubjectAsync(id, request);
            return Ok(new { Message = "Subject updated successfully." });
        }

        /// <summary>
        /// Deletes a subject (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubject(long id)
        {
            await _subjectService.DeleteSubjectAsync(id);
            return Ok(new { Message = "Subject deleted successfully." });
        }



        /// <summary>
        /// Gen the temporarily tip from AI for a subject
        /// </summary>
        [HttpGet("gen-tip/{id}")]
        public async Task<IActionResult> GenTempTipForSubject(long id)
        {
            var res = await _subjectService.GenTempTipForSubjectAsync(id);
            return Ok(res);
        }

    }
}