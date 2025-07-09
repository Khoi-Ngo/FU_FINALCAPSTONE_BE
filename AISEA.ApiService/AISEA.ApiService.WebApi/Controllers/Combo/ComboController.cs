using AISEA.ApiService.BAL.Services.Combo;
using AISEA.ApiService.SHARED.DTOs.Requests.Combo;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.Filters;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.Combo
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComboController : BaseController
    {
        private readonly ComboService _comboService;

        public ComboController(EndpointSettings endpointSettings, ComboService comboService) : base(endpointSettings)
        {
            _comboService = comboService;
        }

        /// <summary>
        /// Creates a new subject combo (Academic Staff only)
        /// </summary>
        [HttpPost]
        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
        public async Task<IActionResult> CreateCombo([FromBody] CreateComboRequest request)
        {
            var comboId = await _comboService.CreateComboAsync(request);
            return Ok(new { Message = "Combo created successfully.", ComboId = comboId });
        }

        /// <summary>
        /// Creates multiple combos in bulk (Academic Staff only)
        /// </summary>
        [HttpPost("bulk")]
        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
        public async Task<IActionResult> CreateCombos([FromBody] List<CreateComboRequest> requests)
        {
            await _comboService.CreateCombosAsync(requests);
            return Ok(new { Message = "Combos created successfully." });
        }

        /// <summary>
        /// Gets paginated list of combos with optional search
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCombos([FromQuery] PaginationRequest request, [FromQuery] string? search = null)
        {
            var result = await _comboService.GetCombosPagedAsync(request, search);
            return Ok(result);
        }

        /// <summary>
        /// Gets combo detail by ID including all subjects
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetComboById(long id)
        {
            var result = await _comboService.GetComboDetailAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing combo (Academic Staff only)
        /// </summary>
        [HttpPut("{id}")]
        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
        public async Task<IActionResult> UpdateCombo(long id, [FromBody] UpdateComboRequest request)
        {
            await _comboService.UpdateComboAsync(id, request);
            return Ok(new { Message = "Combo updated successfully." });
        }

        /// <summary>
        /// Deletes a combo (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [PermissionAuthorize(1)] // Admin only
        public async Task<IActionResult> DeleteCombo(long id)
        {
            await _comboService.DeleteComboAsync(id);
            return Ok(new { Message = "Combo deleted successfully." });
        }

        /// <summary>
        /// Gets all subjects within a combo
        /// </summary>
        [HttpGet("{id}/subjects")]
        public async Task<IActionResult> GetComboSubjects(long id)
        {
            var result = await _comboService.GetComboSubjectsAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Adds a subject to a combo (Academic Staff only)
        /// </summary>
        [HttpPost("{id}/subjects/{subjectId}")]
        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
        public async Task<IActionResult> AddSubjectToCombo(long id, long subjectId)
        {
            await _comboService.AddSubjectToComboAsync(id, subjectId);
            return Ok(new { Message = "Subject added to combo successfully." });
        }

        /// <summary>
        /// Removes a subject from a combo (Academic Staff only)
        /// </summary>
        [HttpDelete("{id}/subjects/{subjectId}")]
        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
        public async Task<IActionResult> RemoveSubjectFromCombo(long id, long subjectId)
        {
            await _comboService.RemoveSubjectFromComboAsync(id, subjectId);
            return Ok(new { Message = "Subject removed from combo successfully." });
        }
    }
}