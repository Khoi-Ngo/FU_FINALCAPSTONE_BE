using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.DASHBOARD
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : BaseController
    {
        private readonly JoinedSubjectForDashboardRepo _joinedSubjectForDashboardRepo;
        private readonly UserForDashboardRepo _userForDashboardRepo;

        public DashboardController(
            EndpointSettings endpointSettings,
            JoinedSubjectForDashboardRepo joinedSubjectForDashboardRepo,
            UserForDashboardRepo userForDashboardRepo)
            : base(endpointSettings)
        {
            _joinedSubjectForDashboardRepo = joinedSubjectForDashboardRepo;
            _userForDashboardRepo = userForDashboardRepo;
        }


        #region UserForDashboardRepo Endpoints

        /// <summary>
        /// User count by status (Pie Chart - Admin).
        /// </summary>
        [HttpGet("users/by-status")]
        public async Task<IActionResult> GetUserCountByStatusAsync()
        {
            var result = await _userForDashboardRepo.GetUserCountByStatusAsync();
            return Ok(result);
        }

        /// <summary>
        /// User count by role (Pie/Bar Chart - Admin).
        /// </summary>
        [HttpGet("users/by-role")]
        public async Task<IActionResult> GetUserCountByRoleAsync()
        {
            var result = await _userForDashboardRepo.GetUserCountByRoleAsync();
            return Ok(result);
        }

        #endregion

        /// <summary>
        /// Retrieves semester performance trend for a specific student, including subjects attempted/passed, credits, and average final score.
        /// </summary>
        /// <param name="studentProfileId">Required: The ID of the student profile to query.</param>
        /// <returns>A list of StudentSemesterPerformanceDto containing semester details, subjects, credits, and average score.</returns>
        [HttpGet("subjects/student-semester-performance")]
        public async Task<IActionResult> GetStudentSemesterPerformanceAsync([FromQuery] long studentProfileId)
        {
            var result = await _joinedSubjectForDashboardRepo.GetStudentSemesterPerformanceAsync(studentProfileId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves assessment category performance breakdown for a specific student, showing average score and total weight per category.
        /// </summary>
        /// <param name="studentProfileId">Required: The ID of the student profile to query.</param>
        /// <returns>A list of StudentCategoryPerformanceDto containing category, average score, and total weight.</returns>
        [HttpGet("subjects/student-category-performance")]
        public async Task<IActionResult> GetStudentCategoryPerformanceAsync([FromQuery] long studentProfileId)
        {
            var result = await _joinedSubjectForDashboardRepo.GetStudentCategoryPerformanceAsync(studentProfileId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves checkpoint status timeline for a specific student, bucketed by month, showing total, completed, and overdue checkpoints.
        /// </summary>
        /// <param name="studentProfileId">Required: The ID of the student profile to query.</param>
        /// <param name="startInclusive">Optional: Start date for filtering checkpoints (inclusive).</param>
        /// <param name="endInclusive">Optional: End date for filtering checkpoints (inclusive).</param>
        /// <returns>A list of StudentCheckpointTimelineDto containing year, month, total, completed, and overdue checkpoint counts.</returns>
        [HttpGet("subjects/student-checkpoint-timeline")]
        public async Task<IActionResult> GetStudentCheckpointTimelineAsync([FromQuery] long studentProfileId, [FromQuery] DateTime? startInclusive = null, [FromQuery] DateTime? endInclusive = null)
        {
            var result = await _joinedSubjectForDashboardRepo.GetStudentCheckpointTimelineAsync(studentProfileId, startInclusive, endInclusive);
            return Ok(result);
        }


        /// <summary>
        /// Retrieves pass rate by semester across all students for admin observation.
        /// </summary>
        /// <returns>A list of AdminSemesterPassRateDto containing semester ID, name, attempted, passed, and pass rate.</returns>
        [HttpGet("subjects/pass-rate-by-semester")]
        public async Task<IActionResult> GetPassRateBySemesterAsync()
        {
            var result = await _joinedSubjectForDashboardRepo.GetPassRateBySemesterAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves average final score by subject code across all semesters and students for admin observation.
        /// </summary>
        /// <returns>A list of AdminAverageScoreBySubjectDto containing subject code, version, average final score, and attempts.</returns>
        [HttpGet("subjects/average-score-by-subject")]
        public async Task<IActionResult> GetAverageScoreBySubjectAsync()
        {
            var result = await _joinedSubjectForDashboardRepo.GetAverageScoreBySubjectAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves overdue checkpoints by semester for admin observation.
        /// </summary>
        /// <returns>A list of AdminOverdueCheckpointBySemesterDto containing semester ID, name, total checkpoints, overdue checkpoints, and overdue rate.</returns>
        [HttpGet("subjects/overdue-checkpoints-by-semester")]
        public async Task<IActionResult> GetOverdueCheckpointsBySemesterAsync()
        {
            var result = await _joinedSubjectForDashboardRepo.GetOverdueCheckpointsBySemesterAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves student risk summary based on low final scores and overdue checkpoints for admin observation.
        /// </summary>
        /// <param name="lowScoreThreshold">Threshold for low final score (default: 60).</param>
        /// <param name="minOverdueCheckpoints">Minimum number of overdue checkpoints to flag as high risk (default: 2).</param>
        /// <returns>A list of AdminStudentRiskDto containing student profile ID, user ID, curriculum, combo code, average score, overdue checkpoints, and risk flags.</returns>
        [HttpGet("subjects/student-risk-summary")]
        public async Task<IActionResult> GetStudentRiskSummaryAsync([FromQuery] double lowScoreThreshold = 60, [FromQuery] int minOverdueCheckpoints = 2)
        {
            var result = await _joinedSubjectForDashboardRepo.GetStudentRiskSummaryAsync(lowScoreThreshold, minOverdueCheckpoints);
            return Ok(result);
        }


    }
}