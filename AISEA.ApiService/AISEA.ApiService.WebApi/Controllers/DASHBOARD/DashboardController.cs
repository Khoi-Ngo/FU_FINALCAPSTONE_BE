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
        private readonly MeetingForDashboardRepo _meetingForDashboardRepo;
        private readonly BookingAvaiForDashboardRepo _bookingAvaiForDashboardRepo;
        private readonly LeaveScheForDashboardRepo _leaveScheForDashboardRepo;
        private readonly JoinedSubjectForDashboardRepo _joinedSubjectForDashboardRepo;
        private readonly UserForDashboardRepo _userForDashboardRepo;

        public DashboardController(
            EndpointSettings endpointSettings,
            MeetingForDashboardRepo meetingForDashboardRepo,
            BookingAvaiForDashboardRepo bookingAvaiForDashboardRepo,
            LeaveScheForDashboardRepo leaveScheForDashboardRepo,
            JoinedSubjectForDashboardRepo joinedSubjectForDashboardRepo,
            UserForDashboardRepo userForDashboardRepo)
            : base(endpointSettings)
        {
            _meetingForDashboardRepo = meetingForDashboardRepo;
            _bookingAvaiForDashboardRepo = bookingAvaiForDashboardRepo;
            _leaveScheForDashboardRepo = leaveScheForDashboardRepo;
            _joinedSubjectForDashboardRepo = joinedSubjectForDashboardRepo;
            _userForDashboardRepo = userForDashboardRepo;
        }

        #region MeetingForDashboardRepo Endpoints

        /// <summary>
        /// Retrieves the count of meetings grouped by status for a pie chart. Admin only.
        /// </summary>
        [HttpGet("meetings/by-status")]
        public async Task<IActionResult> GetMeetingsByStatusAsync()
        {
            var result = await _meetingForDashboardRepo.GetMeetingsByStatusAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves the number of meetings handled per staff. Admin only.
        /// </summary>
        [HttpGet("meetings/staff-load")]
        public async Task<IActionResult> GetStaffMeetingLoadAsync()
        {
            var result = await _meetingForDashboardRepo.GetStaffMeetingLoadAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves meeting trends over the past months. Admin only.
        /// </summary>
        [HttpGet("meetings/trend")]
        public async Task<IActionResult> GetMeetingTrendAsync([FromQuery] int monthsBack = 12)
        {
            var result = await _meetingForDashboardRepo.GetMeetingTrendAsync(monthsBack);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves meeting load for a specific staff member. Staff only.
        /// </summary>
        [HttpGet("meetings/staff/{staffProfileId}/load")]
        public async Task<IActionResult> GetOwnMeetingLoadAsync(long staffProfileId)
        {
            var result = await _meetingForDashboardRepo.GetOwnMeetingLoadAsync(staffProfileId);
            return Ok(result);
        }



        /// <summary>
        /// Retrieves meeting participation for a specific student. Student only.
        /// </summary>
        [HttpGet("meetings/student/{studentProfileId}/participation")]
        public async Task<IActionResult> GetStudentMeetingParticipationAsync(long studentProfileId)
        {
            var result = await _meetingForDashboardRepo.GetStudentMeetingParticipationAsync(studentProfileId);
            return Ok(result);
        }



        #endregion

        #region UserForDashboardRepo Endpoints

        /// <summary>
        /// User count by status (Pie Chart - Admin)
        /// </summary>
        [HttpGet("users/by-status")]
        public async Task<IActionResult> GetUserCountByStatusAsync()
        {
            var result = await _userForDashboardRepo.GetUserCountByStatusAsync();
            return Ok(result);
        }

        /// <summary>
        /// User registration trend over time (Line Chart - Admin)
        /// </summary>
        [HttpGet("users/registration-trend")]
        public async Task<IActionResult> GetUserRegistrationTrendAsync([FromQuery] int monthsBack = 12)
        {
            var result = await _userForDashboardRepo.GetUserRegistrationTrendAsync(monthsBack);
            return Ok(result);
        }

        /// <summary>
        /// Student enrollment by program (Bar Chart - Admin)
        /// </summary>
        [HttpGet("users/students/by-program")]
        public async Task<IActionResult> GetStudentCountByProgramAsync()
        {
            var result = await _userForDashboardRepo.GetStudentCountByProgramAsync();
            return Ok(result);
        }

        /// <summary>
        /// Staff distribution by department (Pie Chart - Admin)
        /// </summary>
        [HttpGet("users/staff/by-department")]
        public async Task<IActionResult> GetStaffCountByDepartmentAsync()
        {
            var result = await _userForDashboardRepo.GetStaffCountByDepartmentAsync();
            return Ok(result);
        }

        #endregion

        #region LeaveScheForDashboardRepo Endpoints

        /// <summary>
        /// Retrieves leave distribution by department (Admin only).
        /// </summary>
        [HttpGet("admin/leaves/by-department")]
        public async Task<IActionResult> GetLeaveByDepartmentAsync()
        {
            var result = await _leaveScheForDashboardRepo.GetLeaveByDepartmentAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves leave distribution by campus (Admin only).
        /// </summary>
        [HttpGet("admin/leaves/by-campus")]
        public async Task<IActionResult> GetLeaveByCampusAsync()
        {
            var result = await _leaveScheForDashboardRepo.GetLeaveByCampusAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves total leave duration for a specific staff (Staff only).
        /// </summary>
        [HttpGet("staff/{staffProfileId}/leaves/duration")]
        public async Task<IActionResult> GetOwnLeaveDurationAsync(long staffProfileId)
        {
            var result = await _leaveScheForDashboardRepo.GetStaffLeaveDurationAsync(staffProfileId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves leave trend over time for a specific staff (Staff only).
        /// </summary>
        [HttpGet("staff/{staffProfileId}/leaves/trend")]
        public async Task<IActionResult> GetOwnLeaveTrendAsync(long staffProfileId, [FromQuery] int monthsBack = 12)
        {
            var result = await _leaveScheForDashboardRepo.GetLeaveTrendAsync(monthsBack, staffProfileId);
            return Ok(result);
        }

        #endregion

        #region BookingAvaiForDashboardRepo Endpoints


        /// <summary>
        /// Retrieves availability slots grouped by department (pie chart).
        /// Admin only.
        /// </summary>
        [HttpGet("admin/availability/by-department")]
        public async Task<IActionResult> GetAvailabilityByDepartmentAsync()
        {
            var result = await _bookingAvaiForDashboardRepo.GetAvailabilityByDepartmentAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves availability slots grouped by campus (bar chart).
        /// Admin only.
        /// </summary>
        [HttpGet("admin/availability/by-campus")]
        public async Task<IActionResult> GetAvailabilityByCampusAsync()
        {
            var result = await _bookingAvaiForDashboardRepo.GetAvailabilityByCampusAsync();
            return Ok(result);
        }

        // ------------------------
        // STAFF ENDPOINTS
        // ------------------------

        /// <summary>
        /// Retrieves availability slots grouped by day of week (pie chart).
        /// Staff-specific.
        /// </summary>
        [HttpGet("staff/{staffProfileId}/availability/by-day")]
        public async Task<IActionResult> GetStaffAvailabilityByDayAsync(long staffProfileId)
        {
            var result = await _bookingAvaiForDashboardRepo.GetStaffAvailabilityByDayAsync(staffProfileId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves total availability hours for a staff (bar chart).
        /// Staff-specific.
        /// </summary>
        [HttpGet("staff/{staffProfileId}/availability/hours")]
        public async Task<IActionResult> GetStaffAvailabilityHoursAsync(long staffProfileId)
        {
            var result = await _bookingAvaiForDashboardRepo.GetStaffAvailabilityHoursAsync(staffProfileId);
            return Ok(result);
        }

        #endregion



        #region JoinedSubjectForDashboardRepo Endpoints
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
        /// Retrieves final score distribution for a specific student, bucketed into configurable ranges (default: 0-50, 50-65, 65-80, 80-90, 90-100).
        /// </summary>
        /// <param name="studentProfileId">Required: The ID of the student profile to query.</param>
        /// <returns>A list of StudentScoreBucketDto containing bucket labels and counts.</returns>
        [HttpGet("subjects/student-score-distribution")]
        public async Task<IActionResult> GetStudentScoreDistributionAsync([FromQuery] long studentProfileId)
        {
            var result = await _joinedSubjectForDashboardRepo.GetStudentScoreDistributionAsync(studentProfileId);
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
        #endregion


    }
}