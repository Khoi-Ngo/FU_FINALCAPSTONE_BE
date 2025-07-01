using AISEA.ApiService.BAL.Services.Combo;
using AISEA.ApiService.SHARED.DTOs.Requests.Combo;
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
        /// Creates a new subject combination with prerequisites validation
        /// </summary>
        [HttpPost]
        [PermissionAuthorize(1, 2, 3)] // Admin, Academic Staff, Advisor
        public async Task<IActionResult> CreateCombo([FromBody] CreateComboRequest request)
        {
            var comboId = await _comboService.CreateComboAsync(request);
            return Ok(new { Message = "Subject combination created successfully.", ComboId = comboId });
        }

        /// <summary>
        /// Gets paginated list of subject combinations with filtering
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCombos([FromQuery] ComboSearchRequest request)
        {
            var result = await _comboService.GetCombosPagedAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Gets detailed information about a subject combination
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetComboById(long id)
        {
            var result = await _comboService.GetComboDetailAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing subject combination
        /// </summary>
        [HttpPut("{id}")]
        [PermissionAuthorize(1, 2, 3)] // Admin, Academic Staff, Advisor
        public async Task<IActionResult> UpdateCombo(long id, [FromBody] UpdateComboRequest request)
        {
            await _comboService.UpdateComboAsync(id, request);
            return Ok(new { Message = "Subject combination updated successfully." });
        }

        /// <summary>
        /// Deletes a subject combination (only if no active enrollments)
        /// </summary>
        [HttpDelete("{id}")]
        [PermissionAuthorize(1, 2)] // Admin, Academic Staff
        public async Task<IActionResult> DeleteCombo(long id)
        {
            await _comboService.DeleteComboAsync(id);
            return Ok(new { Message = "Subject combination deleted successfully." });
        }

        /// <summary>
        /// Checks if a student can enroll in a subject combination
        /// </summary>
        [HttpGet("{id}/availability")]
        public async Task<IActionResult> CheckComboAvailability(long id)
        {
            var result = await _comboService.CheckComboAvailabilityAsync(id, AccessToken);
            return Ok(result);
        }

        /// <summary>
        /// Enrolls a student in a subject combination
        /// </summary>
        [HttpPost("enroll")]
        [PermissionAuthorize(1, 2, 3)] // Admin, Academic Staff, Advisor
        public async Task<IActionResult> EnrollStudent([FromBody] StudentEnrollmentRequest request)
        {
            await _comboService.EnrollStudentAsync(request);
            return Ok(new { Message = "Student enrolled successfully." });
        }

        /// <summary>
        /// Enrolls multiple students in a subject combination
        /// </summary>
        [HttpPost("enroll/bulk")]
        [PermissionAuthorize(1, 2, 3)] // Admin, Academic Staff, Advisor
        public async Task<IActionResult> BulkEnrollStudents([FromBody] BulkEnrollmentRequest request)
        {
            await _comboService.BulkEnrollStudentsAsync(request);
            return Ok(new { Message = "Students enrolled successfully." });
        }

        /// <summary>
        /// Unenrolls a student from a subject combination
        /// </summary>
        [HttpDelete("{comboId}/students/{studentId}")]
        [PermissionAuthorize(1, 2, 3)] // Admin, Academic Staff, Advisor
        public async Task<IActionResult> UnenrollStudent(long comboId, long studentId)
        {
            await _comboService.UnenrollStudentAsync(comboId, studentId);
            return Ok(new { Message = "Student unenrolled successfully." });
        }
    }
}