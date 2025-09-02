using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
        /// Retrieves the count of meetings grouped by status for a pie chart. Intended for admin use.
        /// </summary>
        /// <returns>A list of MeetingByStatus DTOs containing status and meeting count.</returns>
        [HttpGet("meetings/by-status")]
        public async Task<IActionResult> GetMeetingsByStatusAsync()
        {
            var result = await _meetingForDashboardRepo.GetMeetingsByStatusAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves meeting load per staff for a bar chart. Admins see all staff; staff see only their own data.
        /// </summary>
        /// <param name="staffProfileId">Optional: Filters results to a specific staff member if provided.</param>
        /// <returns>A list of StaffMeetingLoad DTOs containing staff name and meeting count.</returns>
        [HttpGet("meetings/staff-load")]
        public async Task<IActionResult> GetStaffMeetingLoadAsync([FromQuery] long? staffProfileId = null)
        {
            var result = await _meetingForDashboardRepo.GetStaffMeetingLoadAsync(staffProfileId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves meeting participation per student for a bar chart. Admins see all students; students see only their own data.
        /// </summary>
        /// <param name="studentProfileId">Optional: Filters results to a specific student if provided.</param>
        /// <returns>A list of StudentMeetingParticipation DTOs containing student name and meeting count.</returns>
        [HttpGet("meetings/student-participation")]
        public async Task<IActionResult> GetStudentMeetingParticipationAsync([FromQuery] long? studentProfileId = null)
        {
            var result = await _meetingForDashboardRepo.GetStudentMeetingParticipationAsync(studentProfileId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves meeting trends over time for a line chart, aggregated by month. Admins see all data; staff/students see their own.
        /// </summary>
        /// <param name="monthsBack">Number of months to look back (default: 12).</param>
        /// <param name="staffProfileId">Optional: Filters results to a specific staff member.</param>
        /// <param name="studentProfileId">Optional: Filters results to a specific student.</param>
        /// <returns>A list of MeetingTrend DTOs containing month and meeting count.</returns>
        [HttpGet("meetings/trend")]
        public async Task<IActionResult> GetMeetingTrendAsync([FromQuery] int monthsBack = 12, [FromQuery] long? staffProfileId = null, [FromQuery] long? studentProfileId = null)
        {
            var result = await _meetingForDashboardRepo.GetMeetingTrendAsync(monthsBack, staffProfileId, studentProfileId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves detailed meeting information for a table. Admins see all meetings; staff/students see their own.
        /// </summary>
        /// <param name="staffProfileId">Optional: Filters results to a specific staff member.</param>
        /// <param name="studentProfileId">Optional: Filters results to a specific student.</param>
        /// <returns>A list of MeetingDetails DTOs containing staff name, student name, start date, status, and issue title.</returns>
        [HttpGet("meetings/details")]
        public async Task<IActionResult> GetMeetingDetailsAsync([FromQuery] long? staffProfileId = null, [FromQuery] long? studentProfileId = null)
        {
            var result = await _meetingForDashboardRepo.GetMeetingDetailsAsync(staffProfileId, studentProfileId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves meeting counts by department for a pie chart. Intended for admin use.
        /// </summary>
        /// <returns>A list of MeetingByDepartment DTOs containing department and meeting count.</returns>
        [HttpGet("meetings/by-department")]
        public async Task<IActionResult> GetMeetingsByDepartmentAsync()
        {
            var result = await _meetingForDashboardRepo.GetMeetingsByDepartmentAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves meeting counts by campus for a bar chart. Intended for admin use.
        /// </summary>
        /// <returns>A list of MeetingByCampus DTOs containing campus and meeting count.</returns>
        [HttpGet("meetings/by-campus")]
        public async Task<IActionResult> GetMeetingsByCampusAsync()
        {
            var result = await _meetingForDashboardRepo.GetMeetingsByCampusAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves total meeting duration per staff for a bar chart. Admins see all staff; staff see their own data.
        /// </summary>
        /// <param name="staffProfileId">Optional: Filters results to a specific staff member.</param>
        /// <returns>A list of MeetingDurationByStaff DTOs containing staff name and total hours.</returns>
        [HttpGet("meetings/duration-by-staff")]
        public async Task<IActionResult> GetMeetingDurationByStaffAsync([FromQuery] long? staffProfileId = null)
        {
            var result = await _meetingForDashboardRepo.GetMeetingDurationByStaffAsync(staffProfileId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves meeting counts by day of week for a bar chart. Admins see all data; staff/students see their own.
        /// </summary>
        /// <param name="staffProfileId">Optional: Filters results to a specific staff member.</param>
        /// <param name="studentProfileId">Optional: Filters results to a specific student.</param>
        /// <returns>A list of MeetingByDayOfWeek DTOs containing day of week and meeting count.</returns>
        [HttpGet("meetings/by-day-of-week")]
        public async Task<IActionResult> GetMeetingsByDayOfWeekAsync([FromQuery] long? staffProfileId = null, [FromQuery] long? studentProfileId = null)
        {
            var result = await _meetingForDashboardRepo.GetMeetingsByDayOfWeekAsync(staffProfileId, studentProfileId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves meeting feedback summary for a table. Admins see all staff; staff see their own data.
        /// </summary>
        /// <param name="staffProfileId">Optional: Filters results to a specific staff member.</param>
        /// <returns>A list of MeetingFeedbackSummary DTOs containing staff name, meetings with feedback, and total meetings.</returns>
        [HttpGet("meetings/feedback-summary")]
        public async Task<IActionResult> GetMeetingFeedbackSummaryAsync([FromQuery] long? staffProfileId = null)
        {
            var result = await _meetingForDashboardRepo.GetMeetingFeedbackSummaryAsync(staffProfileId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves department meeting workload for a table. Intended for admin use.
        /// </summary>
        /// <returns>A list of DepartmentMeetingWorkload DTOs containing department, staff count, and meeting count.</returns>
        [HttpGet("meetings/department-workload")]
        public async Task<IActionResult> GetDepartmentMeetingWorkloadAsync()
        {
            var result = await _meetingForDashboardRepo.GetDepartmentMeetingWorkloadAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves meeting counts by quarter for a line chart. Admins see all data; staff/students see their own.
        /// </summary>
        /// <param name="yearsBack">Number of years to look back (default: 5).</param>
        /// <param name="staffProfileId">Optional: Filters results to a specific staff member.</param>
        /// <param name="studentProfileId">Optional: Filters results to a specific student.</param>
        /// <returns>A list of MeetingByQuarter DTOs containing year, quarter, and meeting count.</returns>
        [HttpGet("meetings/by-quarter")]
        public async Task<IActionResult> GetMeetingsByQuarterAsync([FromQuery] int yearsBack = 5, [FromQuery] long? staffProfileId = null, [FromQuery] long? studentProfileId = null)
        {
            var result = await _meetingForDashboardRepo.GetMeetingsByQuarterAsync(yearsBack, staffProfileId, studentProfileId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves staff profile completeness with meeting count for a table. Admins see all staff; staff see their own data.
        /// </summary>
        /// <param name="staffProfileId">Optional: Filters results to a specific staff member.</param>
        /// <returns>A list of StaffProfileCompleteness DTOs containing staff name, profile completeness percentage, and meeting count.</returns>
        [HttpGet("meetings/staff-profile-completeness")]
        public async Task<IActionResult> GetStaffProfileCompletenessAsync([FromQuery] long? staffProfileId = null)
        {
            var result = await _meetingForDashboardRepo.GetStaffProfileCompletenessAsync(staffProfileId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves meeting counts by campus and department for a stacked bar chart. Intended for admin use.
        /// </summary>
        /// <returns>A list of CampusDepartmentMeetings DTOs containing campus, department, and meeting count.</returns>
        [HttpGet("meetings/campus-department")]
        public async Task<IActionResult> GetCampusDepartmentMeetingsAsync()
        {
            var result = await _meetingForDashboardRepo.GetCampusDepartmentMeetingsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves frequency of student issues discussed in meetings for a table. Admins see all data; students see their own.
        /// </summary>
        /// <param name="studentProfileId">Optional: Filters results to a specific student.</param>
        /// <returns>A list of StudentIssueFrequency DTOs containing issue title and meeting count.</returns>
        [HttpGet("meetings/student-issue-frequency")]
        public async Task<IActionResult> GetStudentIssueFrequencyAsync([FromQuery] long? studentProfileId = null)
        {
            var result = await _meetingForDashboardRepo.GetStudentIssueFrequencyAsync(studentProfileId);
            return Ok(result);
        }
        #endregion

        #region BookingAvaiForDashboardRepo Endpoints
        /// <summary>
        /// Retrieves availability slots by day of week for a pie chart. Admins see all staff; staff see their own data.
        /// </summary>
        /// <param name="staffProfileId">Optional: Filters results to a specific staff member.</param>
        /// <returns>A list of AvailabilityByDay DTOs containing day of week and slot count.</returns>
        [HttpGet("availability/by-day")]
        public async Task<IActionResult> GetAvailabilityByDayAsync([FromQuery] long? staffProfileId = null)
        {
            var result = await _bookingAvaiForDashboardRepo.GetAvailabilityByDayAsync(staffProfileId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves total availability hours per staff for a bar chart. Admins see all staff; staff see their own data.
        /// </summary>
        /// <param name="staffProfileId">Optional: Filters results to a specific staff member.</param>
        /// <returns>A list of StaffAvailabilityHours DTOs containing staff name and total hours.</returns>
        [HttpGet("availability/staff-hours")]
        public async Task<IActionResult> GetStaffAvailabilityHoursAsync([FromQuery] long? staffProfileId = null)
        {
            var result = await _bookingAvaiForDashboardRepo.GetStaffAvailabilityHoursAsync(staffProfileId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves availability slots by department for a pie chart. Intended for admin use.
        /// </summary>
        /// <returns>A list of DepartmentAvailability DTOs containing department and total slots.</returns>
        [HttpGet("availability/by-department")]
        public async Task<IActionResult> GetAvailabilityByDepartmentAsync()
        {
            var result = await _bookingAvaiForDashboardRepo.GetAvailabilityByDepartmentAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves availability slots by campus for a bar chart. Intended for admin use.
        /// </summary>
        /// <returns>A list of CampusAvailabilityDistribution DTOs containing campus and slot count.</returns>
        [HttpGet("availability/by-campus")]
        public async Task<IActionResult> GetAvailabilityByCampusAsync()
        {
            var result = await _bookingAvaiForDashboardRepo.GetAvailabilityByCampusAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves availability trend over time for a line chart, aggregated by month. Admins see all staff; staff see their own data.
        /// </summary>
        /// <param name="monthsBack">Number of months to look back (default: 12).</param>
        /// <param name="staffProfileId">Optional: Filters results to a specific staff member.</param>
        /// <returns>A list of AvailabilityTrend DTOs containing month and slot count.</returns>
        [HttpGet("availability/trend")]
        public async Task<IActionResult> GetAvailabilityTrendAsync([FromQuery] int monthsBack = 12, [FromQuery] long? staffProfileId = null)
        {
            var result = await _bookingAvaiForDashboardRepo.GetAvailabilityTrendAsync(monthsBack, staffProfileId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves detailed staff availability information for a table. Admins see all staff; staff see their own data.
        /// </summary>
        /// <param name="staffProfileId">Optional: Filters results to a specific staff member.</param>
        /// <returns>A list of StaffAvailabilityDetails DTOs containing staff name, day, start time, and end time.</returns>
        [HttpGet("availability/staff-details")]
        public async Task<IActionResult> GetStaffAvailabilityDetailsAsync([FromQuery] long? staffProfileId = null)
        {
            var result = await _bookingAvaiForDashboardRepo.GetStaffAvailabilityDetailsAsync(staffProfileId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves availability slots by position for a bar chart. Intended for admin use.
        /// </summary>
        /// <returns>A list of AvailabilityByPosition DTOs containing position and total slots.</returns>
        [HttpGet("availability/by-position")]
        public async Task<IActionResult> GetAvailabilityByPositionAsync()
        {
            var result = await _bookingAvaiForDashboardRepo.GetAvailabilityByPositionAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves staff availability summary for a table. Admins see all staff; staff see their own data.
        /// </summary>
        /// <param name="staffProfileId">Optional: Filters results to a specific staff member.</param>
        /// <returns>A list of StaffAvailabilitySummary DTOs containing staff name, total slots, and average hours per slot.</returns>
        [HttpGet("availability/staff-summary")]
        public async Task<IActionResult> GetStaffAvailabilitySummaryAsync([FromQuery] long? staffProfileId = null)
        {
            var result = await _bookingAvaiForDashboardRepo.GetStaffAvailabilitySummaryAsync(staffProfileId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves availability by time slot for a bar chart. Admins see all staff; staff see their own data.
        /// </summary>
        /// <param name="staffProfileId">Optional: Filters results to a specific staff member.</param>
        /// <returns>A list of AvailabilityByTimeSlot DTOs containing start time and slot count.</returns>
        [HttpGet("availability/by-time-slot")]
        public async Task<IActionResult> GetAvailabilityByTimeSlotAsync([FromQuery] long? staffProfileId = null)
        {
            var result = await _bookingAvaiForDashboardRepo.GetAvailabilityByTimeSlotAsync(staffProfileId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves staff availability status for a table. Admins see all staff; staff see their own data.
        /// </summary>
        /// <param name="staffProfileId">Optional: Filters results to a specific staff member.</param>
        /// <returns>A list of StaffAvailabilityStatus DTOs containing staff name, active status, and slot count.</returns>
        [HttpGet("availability/staff-status")]
        public async Task<IActionResult> GetStaffAvailabilityStatusAsync([FromQuery] long? staffProfileId = null)
        {
            var result = await _bookingAvaiForDashboardRepo.GetStaffAvailabilityStatusAsync(staffProfileId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves department workload by staff and slots for a table. Intended for admin use.
        /// </summary>
        /// <returns>A list of DepartmentWorkload DTOs containing department, staff count, and total slots.</returns>
        [HttpGet("availability/department-workload")]
        public async Task<IActionResult> GetDepartmentWorkloadAsync()
        {
            var result = await _bookingAvaiForDashboardRepo.GetDepartmentWorkloadAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves availability by quarter for a line chart. Admins see all staff; staff see their own data.
        /// </summary>
        /// <param name="yearsBack">Number of years to look back (default: 5).</param>
        /// <param name="staffProfileId">Optional: Filters results to a specific staff member.</param>
        /// <returns>A list of AvailabilityByQuarter DTOs containing year, quarter, and slot count.</returns>
        [HttpGet("availability/by-quarter")]
        public async Task<IActionResult> GetAvailabilityByQuarterAsync([FromQuery] int yearsBack = 5, [FromQuery] long? staffProfileId = null)
        {
            var result = await _bookingAvaiForDashboardRepo.GetAvailabilityByQuarterAsync(yearsBack, staffProfileId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves staff profile completeness with availability slot count for a table. Admins see all staff; staff see their own data.
        /// </summary>
        /// <param name="staffProfileId">Optional: Filters results to a specific staff member.</param>
        /// <returns>A list of StaffProfileCompleteness DTOs containing staff name, profile completeness percentage, and slot count.</returns>
        [HttpGet("availability/staff-profile-completeness")]
        public async Task<IActionResult> GetAvailabilityStaffProfileCompletenessAsync([FromQuery] long? staffProfileId = null)
        {
            var result = await _bookingAvaiForDashboardRepo.GetStaffProfileCompletenessAsync(staffProfileId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves availability by campus and department for a stacked bar chart. Intended for admin use.
        /// </summary>
        /// <returns>A list of CampusDepartmentAvailability DTOs containing campus, department, and slot count.</returns>
        [HttpGet("availability/campus-department")]
        public async Task<IActionResult> GetCampusDepartmentAvailabilityAsync()
        {
            var result = await _bookingAvaiForDashboardRepo.GetCampusDepartmentAvailabilityAsync();
            return Ok(result);
        }
        #endregion

        #region LeaveScheForDashboardRepo Endpoints
        /// <summary>
        /// Retrieves leave schedules by department for a pie chart. Intended for admin use.
        /// </summary>
        /// <returns>A list of LeaveByDepartment DTOs containing department and leave count.</returns>
        [HttpGet("leaves/by-department")]
        public async Task<IActionResult> GetLeaveByDepartmentAsync()
        {
            var result = await _leaveScheForDashboardRepo.GetLeaveByDepartmentAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves total leave duration per staff for a bar chart. Admins see all staff; staff see their own data.
        /// </summary>
        /// <param name="staffProfileId">Optional: Filters results to a specific staff member.</param>
        /// <returns>A list of StaffLeaveDuration DTOs containing staff name and total days.</returns>
        [HttpGet("leaves/staff-duration")]
        public async Task<IActionResult> GetStaffLeaveDurationAsync([FromQuery] long? staffProfileId = null)
        {
            var result = await _leaveScheForDashboardRepo.GetStaffLeaveDurationAsync(staffProfileId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves leave schedules by campus for a bar chart. Intended for admin use.
        /// </summary>
        /// <returns>A list of LeaveByCampus DTOs containing campus and leave count.</returns>
        [HttpGet("leaves/by-campus")]
        public async Task<IActionResult> GetLeaveByCampusAsync()
        {
            var result = await _leaveScheForDashboardRepo.GetLeaveByCampusAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves leave trend over time for a line chart, aggregated by month. Admins see all staff; staff see their own data.
        /// </summary>
        /// <param name="monthsBack">Number of months to look back (default: 12).</param>
        /// <param name="staffProfileId">Optional: Filters results to a specific staff member.</param>
        /// <returns>A list of LeaveTrend DTOs containing month and leave count.</returns>
        [HttpGet("leaves/trend")]
        public async Task<IActionResult> GetLeaveTrendAsync([FromQuery] int monthsBack = 12, [FromQuery] long? staffProfileId = null)
        {
            var result = await _leaveScheForDashboardRepo.GetLeaveTrendAsync(monthsBack, staffProfileId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves detailed staff leave information for a table. Admins see all staff; staff see their own data.
        /// </summary>
        /// <param name="staffProfileId">Optional: Filters results to a specific staff member.</param>
        /// <returns>A list of StaffLeaveDetails DTOs containing staff name, department, start date, and duration in days.</returns>
        [HttpGet("leaves/staff-details")]
        public async Task<IActionResult> GetStaffLeaveDetailsAsync([FromQuery] long? staffProfileId = null)
        {
            var result = await _leaveScheForDashboardRepo.GetStaffLeaveDetailsAsync(staffProfileId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves leave schedules by position for a pie chart. Intended for admin use.
        /// </summary>
        /// <returns>A list of LeaveByPosition DTOs containing position and leave count.</returns>
        [HttpGet("leaves/by-position")]
        public async Task<IActionResult> GetLeaveByPositionAsync()
        {
            var result = await _leaveScheForDashboardRepo.GetLeaveByPositionAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves staff with the longest leave durations for a table. Admins see all staff; staff see their own data.
        /// </summary>
        /// <param name="staffProfileId">Optional: Filters results to a specific staff member.</param>
        /// <returns>A list of LongLeaveStaff DTOs containing staff name, total days, and leave instances.</returns>
        [HttpGet("leaves/long-leaves")]
        public async Task<IActionResult> GetLongLeaveStaffAsync([FromQuery] long? staffProfileId = null)
        {
            var result = await _leaveScheForDashboardRepo.GetLongLeaveStaffAsync(staffProfileId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves overlapping leave schedules by department for a table. Intended for admin use.
        /// </summary>
        /// <returns>A list of LeaveOverlap DTOs containing department and overlapping leave count.</returns>
        [HttpGet("leaves/overlapping")]
        public async Task<IActionResult> GetOverlappingLeavesAsync()
        {
            var result = await _leaveScheForDashboardRepo.GetOverlappingLeavesAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves leave schedules by day of week for a bar chart. Admins see all staff; staff see their own data.
        /// </summary>
        /// <param name="staffProfileId">Optional: Filters results to a specific staff member.</param>
        /// <returns>A list of LeaveByDayOfWeek DTOs containing day of week and leave count.</returns>
        [HttpGet("leaves/by-day-of-week")]
        public async Task<IActionResult> GetLeaveByDayOfWeekAsync([FromQuery] long? staffProfileId = null)
        {
            var result = await _leaveScheForDashboardRepo.GetLeaveByDayOfWeekAsync(staffProfileId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves staff leave status for a table. Admins see all staff; staff see their own data.
        /// </summary>
        /// <param name="staffProfileId">Optional: Filters results to a specific staff member.</param>
        /// <returns>A list of StaffLeaveStatus DTOs containing staff name, active status, and leave count.</returns>
        [HttpGet("leaves/staff-status")]
        public async Task<IActionResult> GetStaffLeaveStatusAsync([FromQuery] long? staffProfileId = null)
        {
            var result = await _leaveScheForDashboardRepo.GetStaffLeaveStatusAsync(staffProfileId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves department leave workload for a table. Intended for admin use.
        /// </summary>
        /// <returns>A list of DepartmentLeaveWorkload DTOs containing department, staff count, and average leave days.</returns>
        [HttpGet("leaves/department-workload")]
        public async Task<IActionResult> GetDepartmentLeaveWorkloadAsync()
        {
            var result = await _leaveScheForDashboardRepo.GetDepartmentLeaveWorkloadAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves leave schedules by quarter for a line chart. Admins see all staff; staff see their own data.
        /// </summary>
        /// <param name="yearsBack">Number of years to look back (default: 5).</param>
        /// <param name="staffProfileId">Optional: Filters results to a specific staff member.</param>
        /// <returns>A list of LeaveByQuarter DTOs containing year, quarter, and leave count.</returns>
        [HttpGet("leaves/by-quarter")]
        public async Task<IActionResult> GetLeaveByQuarterAsync([FromQuery] int yearsBack = 5, [FromQuery] long? staffProfileId = null)
        {
            var result = await _leaveScheForDashboardRepo.GetLeaveByQuarterAsync(yearsBack, staffProfileId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves staff profile completeness with leave count for a table. Admins see all staff; staff see their own data.
        /// </summary>
        /// <param name="staffProfileId">Optional: Filters results to a specific staff member.</param>
        /// <returns>A list of StaffProfileCompleteness DTOs containing staff name, profile completeness percentage, and leave count.</returns>
        [HttpGet("leaves/staff-profile-completeness")]
        public async Task<IActionResult> GetLeaveStaffProfileCompletenessAsync([FromQuery] long? staffProfileId = null)
        {
            var result = await _leaveScheForDashboardRepo.GetStaffProfileCompletenessAsync(staffProfileId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves leave schedules by campus and department for a stacked bar chart. Intended for admin use.
        /// </summary>
        /// <returns>A list of CampusDepartmentLeave DTOs containing campus, department, and leave count.</returns>
        [HttpGet("leaves/campus-department")]
        public async Task<IActionResult> GetCampusDepartmentLeaveAsync()
        {
            var result = await _leaveScheForDashboardRepo.GetCampusDepartmentLeaveAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves total leave duration by year for a line chart. Admins see all staff; staff see their own data.
        /// </summary>
        /// <param name="yearsBack">Number of years to look back (default: 5).</param>
        /// <param name="staffProfileId">Optional: Filters results to a specific staff member.</param>
        /// <returns>A list of LeaveDurationByYear DTOs containing year and total days.</returns>
        [HttpGet("leaves/duration-by-year")]
        public async Task<IActionResult> GetLeaveDurationByYearAsync([FromQuery] int yearsBack = 5, [FromQuery] long? staffProfileId = null)
        {
            var result = await _leaveScheForDashboardRepo.GetLeaveDurationByYearAsync(yearsBack, staffProfileId);
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

        #region UserForDashboardRepo Endpoints
        /// <summary>
        /// Retrieves user count by status for a pie chart.
        /// </summary>
        /// <returns>A dictionary mapping EUserStatus to user counts.</returns>
        [HttpGet("users/by-status")]
        public async Task<IActionResult> GetUserCountByStatusAsync()
        {
            var result = await _userForDashboardRepo.GetUserCountByStatusAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves user registration trend over time for a line chart, aggregated by month.
        /// </summary>
        /// <param name="monthsBack">Number of months to look back (default: 12).</param>
        /// <returns>A list of UserRegistrationTrend DTOs containing month and user count.</returns>
        [HttpGet("users/registration-trend")]
        public async Task<IActionResult> GetUserRegistrationTrendAsync([FromQuery] int monthsBack = 12)
        {
            var result = await _userForDashboardRepo.GetUserRegistrationTrendAsync(monthsBack);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves student enrollment by program for a bar chart.
        /// </summary>
        /// <returns>A list of StudentProgramCount DTOs containing program name and student count.</returns>
        [HttpGet("users/student-by-program")]
        public async Task<IActionResult> GetStudentCountByProgramAsync()
        {
            var result = await _userForDashboardRepo.GetStudentCountByProgramAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves staff distribution by department for a pie chart.
        /// </summary>
        /// <returns>A dictionary mapping department names to staff counts.</returns>
        [HttpGet("users/staff-by-department")]
        public async Task<IActionResult> GetStaffCountByDepartmentAsync()
        {
            var result = await _userForDashboardRepo.GetStaffCountByDepartmentAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves active students with bans for a table.
        /// </summary>
        /// <returns>A list of StudentBanInfo DTOs containing student name and number of bans.</returns>
        [HttpGet("users/students-with-bans")]
        public async Task<IActionResult> GetStudentsWithBansAsync()
        {
            var result = await _userForDashboardRepo.GetStudentsWithBansAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves average age of users by role for a bar chart.
        /// </summary>
        /// <returns>A list of RoleAgeInfo DTOs containing role name and average age.</returns>
        [HttpGet("users/average-age-by-role")]
        public async Task<IActionResult> GetAverageAgeByRoleAsync()
        {
            var result = await _userForDashboardRepo.GetAverageAgeByRoleAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves the top 5 active programs by student enrollment for a table.
        /// </summary>
        /// <returns>A list of TopProgramInfo DTOs containing program name, student count, and latest enrollment date.</returns>
        [HttpGet("users/top-programs")]
        public async Task<IActionResult> GetTopActiveProgramsAsync()
        {
            var result = await _userForDashboardRepo.GetTopActiveProgramsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves staff tenure by years of service for a bar chart.
        /// </summary>
        /// <returns>A list of StaffTenureInfo DTOs containing staff name and years of service.</returns>
        [HttpGet("users/staff-tenure")]
        public async Task<IActionResult> GetStaffTenureAsync()
        {
            var result = await _userForDashboardRepo.GetStaffTenureAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves user role distribution for a pie chart.
        /// </summary>
        /// <returns>A list of UserRoleDistribution DTOs containing role name and user count.</returns>
        [HttpGet("users/role-distribution")]
        public async Task<IActionResult> GetUserRoleDistributionAsync()
        {
            var result = await _userForDashboardRepo.GetUserRoleDistributionAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves student enrollment by year for a line chart.
        /// </summary>
        /// <returns>A list of StudentEnrollmentByYear DTOs containing year and student count.</returns>
        [HttpGet("users/student-enrollment-by-year")]
        public async Task<IActionResult> GetStudentEnrollmentByYearAsync()
        {
            var result = await _userForDashboardRepo.GetStudentEnrollmentByYearAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves staff department workload summary for a table.
        /// </summary>
        /// <returns>A list of StaffDepartmentWorkload DTOs containing department, staff count, and average years of service.</returns>
        [HttpGet("users/staff-department-workload")]
        public async Task<IActionResult> GetStaffDepartmentWorkloadAsync()
        {
            var result = await _userForDashboardRepo.GetStaffDepartmentWorkloadAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves user activity summary for a table, showing profile completeness.
        /// </summary>
        /// <returns>A list of UserActivitySummary DTOs containing user name, role name, and profile completeness percentage.</returns>
        [HttpGet("users/activity-summary")]
        public async Task<IActionResult> GetUserActivitySummaryAsync()
        {
            var result = await _userForDashboardRepo.GetUserActivitySummaryAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves student career goal distribution for a pie chart.
        /// </summary>
        /// <returns>A list of StudentCareerGoalDistribution DTOs containing career goal and student count.</returns>
        [HttpGet("users/student-career-goal-distribution")]
        public async Task<IActionResult> GetStudentCareerGoalDistributionAsync()
        {
            var result = await _userForDashboardRepo.GetStudentCareerGoalDistributionAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves staff campus distribution for a bar chart.
        /// </summary>
        /// <returns>A list of StaffCampusDistribution DTOs containing campus and staff count.</returns>
        [HttpGet("users/staff-campus-distribution")]
        public async Task<IActionResult> GetStaffCampusDistributionAsync()
        {
            var result = await _userForDashboardRepo.GetStaffCampusDistributionAsync();
            return Ok(result);
        }

        /// <summary>
        /// Retrieves user creation by quarter for a line chart.
        /// </summary>
        /// <param name="yearsBack">Number of years to look back (default: 5).</param>
        /// <returns>A list of UserCreationByQuarter DTOs containing year, quarter, and user count.</returns>
        [HttpGet("users/creation-by-quarter")]
        public async Task<IActionResult> GetUserCreationByQuarterAsync([FromQuery] int yearsBack = 5)
        {
            var result = await _userForDashboardRepo.GetUserCreationByQuarterAsync(yearsBack);
            return Ok(result);
        }
        #endregion
    }
}