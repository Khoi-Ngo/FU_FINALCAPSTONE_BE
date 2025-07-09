using AISEA.ApiService.BAL.Services.Program;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.DTOs.Requests.Program;
using AISEA.ApiService.SHARED.Filters;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.Program
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgramController : BaseController
    {
        private readonly ProgramService _programService;

        public ProgramController(EndpointSettings endpointSettings, ProgramService programService) : base(endpointSettings)
        {
            _programService = programService;
        }

        /// <summary>
        /// Creates a new program (Academic Staff only)
        /// </summary>
        [HttpPost]
        [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.ACADEMIC_STAFF)]
        public async Task<IActionResult> CreateProgram([FromBody] CreateProgramRequest request)
        {
            var programId = await _programService.CreateProgramAsync(request);
            return Ok(new { Message = "Program created successfully.", ProgramId = programId });
        }

        /// <summary>
        /// Creates multiple programs in bulk (Academic Staff only)
        /// </summary>
        [HttpPost("bulk")]
        [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.ACADEMIC_STAFF)]
        public async Task<IActionResult> CreatePrograms([FromBody] List<CreateProgramRequest> requests)
        {
            await _programService.CreateProgramsAsync(requests);
            return Ok(new { Message = "Programs created successfully." });
        }

        /// <summary>
        /// Gets paginated list of programs with optional search
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPrograms([FromQuery] PaginationRequest request, [FromQuery] string? search = null)
        {
            var result = await _programService.GetProgramsPagedAsync(request, search);
            return Ok(result);
        }

        /// <summary>
        /// Gets all active programs (for dropdowns)
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetActivePrograms()
        {
            var result = await _programService.GetAllActiveProgramsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Gets a program by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProgramById(long id)
        {
            var result = await _programService.GetProgramByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing program (Academic Staff only)
        /// </summary>
        [HttpPut("{id}")]
        [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.ACADEMIC_STAFF)]
        public async Task<IActionResult> UpdateProgram(long id, [FromBody] UpdateProgramRequest request)
        {
            await _programService.UpdateProgramAsync(id, request);
            return Ok(new { Message = "Program updated successfully." });
        }

        /// <summary>
        /// Deletes a program (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [PermissionAuthorize((int)EUserRole.ADMIN)]
        public async Task<IActionResult> DeleteProgram(long id)
        {
            await _programService.DeleteProgramAsync(id);
            return Ok(new { Message = "Program deleted successfully." });
        }
    }
}
