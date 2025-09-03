using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.Booking;
using AISEA.ApiService.SHARED.DTOs.Requests.Noti;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.DTOs.Responses.Booking;
using AISEA.ApiService.SHARED.DTOs.Responses.Pagin;
using AISEA.ApiService.SHARED.Exceptions;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.SHARED.PropConfigs;
using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NanoidDotNet;

namespace AISEA.ApiService.BAL.Services.Booking;

public class BookedMeetingService
{

    #region Init

    private readonly BookedMeetingRepository _bookedMeetingRepository;
    private readonly StaffProfileRepository _staffProfileRepository;
    private readonly StudentProfileRepository _studentProfileRepository;
    private readonly IJWTService _jWTService;
    private readonly IHolidayService _holidayService;
    private readonly IMapper _mapper;
    private readonly BookingSettings _bookingSettings;
    private readonly IMailService _mailService;

    public BookedMeetingService(BookedMeetingRepository bookedMeetingRepository, StaffProfileRepository staffProfileRepository, StudentProfileRepository studentProfileRepository, IJWTService jWTService, IHolidayService holidayService, IMapper mapper, BookingSettings bookingSettings, IMailService mailService)
    {
        _bookedMeetingRepository = bookedMeetingRepository;
        _staffProfileRepository = staffProfileRepository;
        _studentProfileRepository = studentProfileRepository;
        _jWTService = jWTService;
        _holidayService = holidayService;
        _mapper = mapper;
        _bookingSettings = bookingSettings;
        _mailService = mailService;
    }

    #endregion

    #region GET MULTIPLE

    public object? GetMaxNumberOfBan()
    {
        return new
        {
            MaxNoOfBan = _bookingSettings.MaxNumberOfBan
        };
    }
    public async Task<object?> GetCurNumberOfBanAsync(string accessToken)
    {
        var studentProfile = await _studentProfileRepository.GetByIdAsync(_jWTService.GetProfileIdFromToken(accessToken));
        return new
        {
            CurNoOfBan = studentProfile.NumberOfBan
        };
    }
    public async Task<PagedResult<MeetingItemListResponse>> GetAllActiveByStudentSelfAsync(PaginationRequest request, string accessToken)
    {
        var studentProfileId = _jWTService.GetProfileIdFromToken(accessToken);
        var (meetings, totalCount) = await _bookedMeetingRepository.GetAllActiveByStudentProfileIdAsync(request, studentProfileId);
        return new PagedResult<MeetingItemListResponse>
        {
            Items = _mapper.Map<List<MeetingItemListResponse>>(meetings),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
    public async Task<PagedResult<MeetingItemListResponse>> GetAllActiveByAdvSelfAsync(PaginationRequest request, string accessToken)
    {
        var staffProfileId = _jWTService.GetProfileIdFromToken(accessToken);
        var (meetings, totalCount) = await _bookedMeetingRepository.GetAllActiveByStaffProfileIdAsync(request, staffProfileId);
        return new PagedResult<MeetingItemListResponse>
        {
            Items = _mapper.Map<List<MeetingItemListResponse>>(meetings),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }


    public async Task<List<OverdueMeetingDTO>> GetPendingOverdueMeetingsWithUserIdsAsync()
    {
        return await _bookedMeetingRepository.GetPendingOverdueMeetingsWithUserIdsAsync();
    }


    public async Task<List<StuMissedMeetingDTO>> GetConfirmedStudentMissedMeetingsAsync(int daysToCheckStudentMissedAfterEndMeeting)
    {
        return await _bookedMeetingRepository.GetConfirmedStudentMissedMeetingsAsync(daysToCheckStudentMissedAfterEndMeeting);
    }

    public async Task<PagedResult<MeetingItemListResponse>> GetAllAsync(PaginationRequest request)
    {
        var (meetings, totalCount) = await _bookedMeetingRepository.GetAllAsync(request);
        return new PagedResult<MeetingItemListResponse>
        {
            Items = _mapper.Map<List<MeetingItemListResponse>>(meetings),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<PagedResult<MeetingItemListResponse>> GetAllByStudentSelfAsync(PaginationRequest request, string accessToken)
    {
        var studentProfileId = _jWTService.GetProfileIdFromToken(accessToken);
        var (meetings, totalCount) = await _bookedMeetingRepository.GetAllByStudentProfileIdAsync(request, studentProfileId);
        return new PagedResult<MeetingItemListResponse>
        {
            Items = _mapper.Map<List<MeetingItemListResponse>>(meetings),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<PagedResult<MeetingItemListResponse>> GetAllByAdvSelfAsync(PaginationRequest request, string accessToken)
    {
        var staffProfileId = _jWTService.GetProfileIdFromToken(accessToken);
        var (meetings, totalCount) = await _bookedMeetingRepository.GetAllByStaffProfileIdAsync(request, staffProfileId);
        return new PagedResult<MeetingItemListResponse>
        {
            Items = _mapper.Map<List<MeetingItemListResponse>>(meetings),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<PagedResult<MeetingItemListResponse>> GetAllByStaffProfileIdForStudentRoleAsync(PaginationRequest request, long staffProfileId)
    {
        var (meetings, totalCount) = await _bookedMeetingRepository.GetAllActiveByStaffProfileIdAsync(request, staffProfileId);
        return new PagedResult<MeetingItemListResponse>
        {
            Items = _mapper.Map<List<MeetingItemListResponse>>(meetings),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    #endregion



    public async Task StuCancelPendingAsync(long id, NoteDTO request, string accessToken)
    {

        try
        {
            //simply shift the status to the STU_CANCELED without any BAN (The mechanism anti spam will be on the worker service)
            var pendingMeeting = await _bookedMeetingRepository.GetByIdAsync(id);

            if (pendingMeeting.Status != EBookingStatus.PENDING) throw new InvalidCurMeetingStatException("Cannot execute command on the meeting if the status of meeting is not current " + EBookingStatus.PENDING.ToString());

            //!Disable the realtime check
            // if (DateTime.Now > pendingMeeting.StartDateTime) throw new InvalidOperationException($"No need to cancel the meeting with status = {EBookingStatus.PENDING.ToString()} when the current time exceed the StartTime of the meeting");

            // if (!IsValidAccess(pendingMeeting, _jWTService.GetRoleIdFromToken(accessToken), _jWTService.GetProfileIdFromToken(accessToken))) throw new InvalidAccessMeeting("Deny permission");

            pendingMeeting.Note = request.Note;
            pendingMeeting.Status = EBookingStatus.STU_CANCELED;

            await _bookedMeetingRepository.UpdateAsync(pendingMeeting);
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
            {
                HandleMeetingSqlException(sqlEx);

            }
            throw;
        }
        catch (SqlException ex)
        {
            HandleMeetingSqlException(ex);
            throw;

        }

    }

    public async Task<(NotificationDTO parterNoti, long partnerUserId)> CompleteAsync(string accessToken, long id, InputCheckinRequest request)
    {

        try
        {
            //validate the time to do + current status + validate access + check in code (trigger)

            var (completedMeeting, studentUserId) = await _bookedMeetingRepository.GetMeetingWithStudentUserIdAsync(id);

            if (!IsValidAccess(completedMeeting, _jWTService.GetRoleIdFromToken(accessToken), _jWTService.GetProfileIdFromToken(accessToken))) throw new InvalidAccessBookingAvailability("Deny access to the meeting");

            //!Disable the realtime check
            if (!(completedMeeting.Status == EBookingStatus.CONFIRMED
            // && DateTime.Now > completedMeeting.StartDateTime
            // && request.CheckInCode == completedMeeting.CheckInCode
            )) throw new InvalidOperationException("Too soon to complete this meeting or the status/ checkin code of this meeting not true");

            completedMeeting.Status = EBookingStatus.COMPLETED;
            await _bookedMeetingRepository.UpdateAsync(completedMeeting);

            return (
                new NotificationDTO
                {
                    Title = completedMeeting.Status.ToString(),
                    Content = $"The meeting {completedMeeting.StartDateTime} - {completedMeeting.EndDateTime} has been {completedMeeting.Status.ToString()}"
                }, partnerUserId: studentUserId
            );
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
            {
                HandleMeetingSqlException(sqlEx);

            }
            throw;
        }
        catch (SqlException ex)
        {
            HandleMeetingSqlException(ex);
            throw;

        }

    }

    public async Task<(NotificationDTO partnerNoti, long partnerUserId)> ConfirmMeetingAsync(long id, string accessToken)
    {
        try
        {
            //validate time + status + access permission
            var (confirmedMeeting, studentUserId) = await _bookedMeetingRepository.GetMeetingWithStudentUserIdAsync(id);

            if (!IsValidAccess(confirmedMeeting, _jWTService.GetRoleIdFromToken(accessToken), _jWTService.GetProfileIdFromToken(accessToken))) throw new InvalidAccessMeeting("You have no permission to confirm this meeting");

            //check the status
            if (confirmedMeeting.Status != EBookingStatus.PENDING)
                throw new InvalidOperationException("The status of this meeting not true");

            //!Disable realtime validation
            // var daysGap = GetTheTimeGap(confirmedMeeting.StartDateTime, DateTime.Now).TotalDays;

            // if (DateTime.Now >= confirmedMeeting.StartDateTime
            // || daysGap < _bookingSettings.MinTimeAdvConfirmOrCancelMeetingDays
            // )
            //     throw new InvalidOperationException("Too late to confirm this meeting");



            confirmedMeeting.Status = EBookingStatus.CONFIRMED;
            await _bookedMeetingRepository.UpdateAsync(confirmedMeeting);

            return (
                 new NotificationDTO
                 {
                     Title = confirmedMeeting.Status.ToString(),
                     Content = $"The meeting {confirmedMeeting.StartDateTime} - {confirmedMeeting.EndDateTime} has been {confirmedMeeting.Status.ToString()}"
                 }, partnerUserId: studentUserId
             );
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
            {
                HandleMeetingSqlException(sqlEx);

            }
            throw;
        }
        catch (SqlException ex)
        {
            HandleMeetingSqlException(ex);
            throw;

        }

    }

    public async Task<(NotificationDTO partnerNoti, long partnerUserId)> CreateMeetingAsync(CreateMeetingRequest request, string accessToken)
    {
        try
        {
            //avoid book the meeting on holiday
            var checkHolidays = await _holidayService.CheckHolidayAsync(DateOnly.FromDateTime(request.StartDateTime));
            if (checkHolidays.Any()) throw new OnHolidayException("You cannot book a meeting on Holiday (VN)", checkHolidays);

            var newMeeting = _mapper.Map<BookedMeeting>(request);
            newMeeting.StudentProfileId = _jWTService.GetProfileIdFromToken(accessToken);

            //exactly matching the booking availability (Trigger)

            //not matching any leave schedule (Trigger)

            /*valid data in booked meeting table + number of Ban in student profile table(Trigger)
            
            - Only one student book then everything good

            - More than one || Rebook then check all existed at STAT = STUDENT_CANCELED ? good : exception

            - Before 2 days to go (both trigger and fluentValidator)

            */


            //generate the check-in code for the student then send via mail
            var checkinCode = Nanoid.Generate(size: 8);



            newMeeting.CheckInCode = checkinCode;

            //save into the database without caching
            await _bookedMeetingRepository.CreateAsync(newMeeting);

            var advisorUserId = await _staffProfileRepository.GetUserIdByIdAsync(request.StaffProfileId);
            await _mailService.SendEmailAsync(_jWTService.GetEmailFromToken(accessToken), "CHECK IN CODE", $"The check in code for your meeting {request.StartDateTime} to {request.EndDateTime} is : {checkinCode}");
            //return the notified advisor user id
            return (
                new NotificationDTO
                {
                    Title = EBookingStatus.PENDING.ToString(),
                    Content = $"The meeting {request.StartDateTime} - {request.EndDateTime} created"
                }, partnerUserId: advisorUserId
            );
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
            {
                HandleMeetingSqlException(sqlEx);

            }
            throw;
        }
        catch (SqlException ex)
        {
            HandleMeetingSqlException(ex);
            throw;

        }

    }
    public async Task DeleteAsync(long id)
    {
        try
        {
            var removedMeeting = await _bookedMeetingRepository.GetByIdAsync(id);
            await _bookedMeetingRepository.RemoveAsync(removedMeeting);
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
            {
                HandleMeetingSqlException(sqlEx);

            }
            throw;
        }
        catch (SqlException ex)
        {
            HandleMeetingSqlException(ex);
            throw;
        }
    }

    public async Task<(NotificationDTO partnerNoi, long partnerUserId)> AdvisorCancelMeetingAsync(string accessToken, NoteDTO request, long meetingId)
    {
        try
        {
            var (canceledMeeting, studentUserId) = await _bookedMeetingRepository.GetMeetingWithStudentUserIdAsync(meetingId);
            //validate the access to meeting
            // if (!IsValidAccess(canceledMeeting, _jWTService.GetRoleIdFromToken(accessToken), _jWTService.GetProfileIdFromToken(accessToken)))
                // throw new InvalidAccessMeeting("No permission to cancel this meeting");

            //check status
            if (!(canceledMeeting.Status == EBookingStatus.CONFIRMED || canceledMeeting.Status == EBookingStatus.PENDING))
                throw new InvalidOperationException("The status is not true for processing the action");

            //check the gap if too late then deny => overdue(FU admin internally observe then giving real-life penalties)
            //! Disable realtime check
            // if (DateTime.Now >= canceledMeeting.StartDateTime || GetTheTimeGap(canceledMeeting.StartDateTime, DateTime.Now).TotalDays < _bookingSettings.MinTimeAdvConfirmOrCancelMeetingDays)
            //     throw new InvalidOperationException("Too late to cancel the meeting, this will be shifted to OVERDUE soon");

            //save to the database
            canceledMeeting.Note = request.Note;
            canceledMeeting.Status = EBookingStatus.ADV_CANCELED;

            await _bookedMeetingRepository.UpdateAsync(canceledMeeting);

            return (
                 new NotificationDTO
                 {
                     Title = canceledMeeting.Status.ToString(),
                     Content = $"The meeting {canceledMeeting.StartDateTime} - {canceledMeeting.EndDateTime} has been {canceledMeeting.Status.ToString()}"
                 }, partnerUserId: studentUserId
             );

        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
            {
                HandleMeetingSqlException(sqlEx);

            }
            throw;
        }
        catch (SqlException ex)
        {
            HandleMeetingSqlException(ex);
            throw;
        }

    }

    public async Task FeedbackAsync(string accessToken, FeedbackRequest request, long meetingId)
    {

        try
        {
            var meeting = await _bookedMeetingRepository.GetByIdAsync(meetingId);
            //validate the access meeting
            // if (!IsValidAccess(meeting, _jWTService.GetRoleIdFromToken(accessToken), _jWTService.GetProfileIdFromToken(accessToken)))
                // throw new InvalidAccessMeeting("No permission to give feedback for this meeting");

            //validate the time + the status ACTIVE but END OF PHASE
            //!Disable realtime check
            // if (DateTime.Now <= meeting.EndDateTime) throw new InvalidOperationException("You cannot give the feedback when the meeting is not over");

            if (meeting.Status != EBookingStatus.COMPLETED && meeting.Status != EBookingStatus.STUDENT_MISSED && meeting.Status != EBookingStatus.ADVISOR_MISSED)
                throw new InvalidOperationException("Cannot give the feedback when the stat of the meeting not comes to the end stat YET");

            meeting.Feedback = request.Feedback;
            meeting.SuggestionFromAdvisor = request.SuggestionFromAdvisor;

            await _bookedMeetingRepository.UpdateAsync(meeting);
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
            {
                HandleMeetingSqlException(sqlEx);

            }
            throw;
        }
        catch (SqlException ex)
        {
            HandleMeetingSqlException(ex);
            throw;
        }
    }


    public async Task<(NotificationDTO partnerNoti, long partnerUserId)> MarkAdvisorMissedAsync(string accessToken, long meetingId, NoteDTO request)
    {
        try
        {
            var (confirmedMeeting, advisorUserId) = await _bookedMeetingRepository.GetMeetingWithAdvisorUserIdAsync(meetingId);
            //valid access the meeting
            // if (!IsValidAccess(confirmedMeeting, _jWTService.GetRoleIdFromToken(accessToken), _jWTService.GetProfileIdFromToken(accessToken)))
                // throw new InvalidAccessMeeting("No permission to report the Advisor missed this meeting");

            //check the stat is CONFIRM
            if (confirmedMeeting.Status != EBookingStatus.CONFIRMED)
                throw new InvalidOperationException("Cannot report the Advisor missed this meeting when the stat of the meeting is NOT CONFIRMED");

            //check the time after start = _bookingSettings.MinLateTimeMinOfAdv
            //!Disable the realtime check
            // if (!(DateTime.Now > confirmedMeeting.StartDateTime
            // && GetTheTimeGap(DateTime.Now, confirmedMeeting.StartDateTime).TotalMinutes >= _bookingSettings.MaxLateTimeForAdvToMeetingMins))
            //     throw new InvalidOperationException($"Not appropriate time for reporting Advisor missing the meeting, make sure the current time a head of StartTime about ${_bookingSettings.MaxLateTimeForAdvToMeetingMins} minutes");

            confirmedMeeting.Note = request.Note;
            confirmedMeeting.Status = EBookingStatus.ADVISOR_MISSED;
            await _bookedMeetingRepository.UpdateAsync(confirmedMeeting);

            return (
                 new NotificationDTO
                 {
                     Title = confirmedMeeting.Status.ToString(),
                     Content = $"The meeting {confirmedMeeting.StartDateTime} - {confirmedMeeting.EndDateTime} has been {confirmedMeeting.Status.ToString()}"
                 }, partnerUserId: advisorUserId
             );

        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
            {
                HandleMeetingSqlException(sqlEx);

            }
            throw;
        }
        catch (SqlException ex)
        {
            HandleMeetingSqlException(ex);
            throw;
        }

    }

    public async Task<(NotificationDTO partnerNoti, long partnerUserId, long studentProfileId, int numberOfBan)> StuCancelTheConfirmedAsync(long meetingId, NoteDTO request, string accessToken)
    {

        try
        {
            var (canceledMeeting, advisorUserId) = await _bookedMeetingRepository.GetMeetingWithAdvisorUserIdAsync(meetingId);
            // -Validate the access
            // if (!IsValidAccess(canceledMeeting, _jWTService.GetRoleIdFromToken(accessToken), _jWTService.GetProfileIdFromToken(accessToken)))
                // throw new InvalidAccessMeeting("No permission to cancel this meeting");
            // -Validate the status
            if (canceledMeeting.Status != EBookingStatus.CONFIRMED) throw new InvalidOperationException("Cannot process with NON-CONFIRMED meeting");

            // -Validate the time, the time gap btw Current - StartTime must be >= Min TimeGap allowing to Cancel
            //!Disable the realtime check
            // var dayGaps = GetTheTimeGap(DateTime.Now, canceledMeeting.StartDateTime).TotalDays;

            // if (DateTime.Now >= canceledMeeting.StartDateTime || dayGaps < _bookingSettings.MinTimeStudentCancelTheConfirmMeetingDays)
            //     throw new InvalidOperationException("Too late to cancel the confirm meeting");

            /*
                    -If cancel -> Number of Ban + 2 if cannot due to too late to do then later the Meeting will be set as StudentMissed and Number of Ban + 3

                    -Happy case: update the status, note of the meeting then return the number of ban = 2
            */
            canceledMeeting.Note = request.Note;
            canceledMeeting.Status = EBookingStatus.STU_CANCELED;
            await _bookedMeetingRepository.UpdateAsync(canceledMeeting);


            return (
                 new NotificationDTO
                 {
                     Title = canceledMeeting.Status.ToString(),
                     Content = $"The meeting {canceledMeeting.StartDateTime} - {canceledMeeting.EndDateTime} has been {canceledMeeting.Status.ToString()}"
                 }, partnerUserId: advisorUserId, studentProfileId: _jWTService.GetProfileIdFromToken(accessToken), _bookingSettings.NumberOfBanWhenStuCancelTheConfirm
             );
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
            {
                HandleMeetingSqlException(sqlEx);

            }
            throw;
        }
        catch (SqlException ex)
        {
            HandleMeetingSqlException(ex);
            throw;
        }

    }
    public async Task<(NotificationDTO partnerNoti, long partnerUserId)> AddReasonForOverdueAsync(string accessToken, NoteDTO request, long meetingId)
    {

        try
        {
            var (overdueMeeting, studentUserId) = await _bookedMeetingRepository.GetMeetingWithStudentUserIdAsync(meetingId);
            //validate the access
            // if (!IsValidAccess(overdueMeeting, _jWTService.GetRoleIdFromToken(accessToken), _jWTService.GetProfileIdFromToken(accessToken)))
                // throw new InvalidAccessMeeting("No permission to give the reason for this overdue meeting");

            //validate the status is overdue or not
            if (overdueMeeting.Status != EBookingStatus.OVERDUE) throw new InvalidOperationException("Cannot process with NON-OVERDUE meeting");

            //giving the reason for overdue
            overdueMeeting.Note = request.Note;
            await _bookedMeetingRepository.UpdateAsync(overdueMeeting);

            return (
                        new NotificationDTO
                        {
                            Title = "Update",
                            Content = $"The meeting {overdueMeeting.StartDateTime} - {overdueMeeting.EndDateTime} has been updated reason"
                        }, partnerUserId: studentUserId
                    );
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
            {
                HandleMeetingSqlException(sqlEx);

            }
            throw;
        }
        catch (SqlException ex)
        {
            HandleMeetingSqlException(ex);
            throw;
        }

    }

    public async Task UpdateMeetingStatusesAsync(List<long> meetingIds, EBookingStatus status)
    {
        await _bookedMeetingRepository.UpdateMeetingStatusesAsync(meetingIds, status);
    }





    #region Private methods
    private void HandleMeetingSqlException(SqlException ex)
    {
        switch (ex.Number)
        {
            case 50006:
                throw new InvalidOperationException("Student has reached the maximum number of bans . Cannot book meeting." + ex.Message);
            case 50007:
                throw new InvalidOperationException("The meeting time conflicts with staff's leave schedule." + ex.Message);
            case 50008:
                throw new InvalidOperationException("The meeting time does not exactly match staff's booking availability." + ex.Message);
            case 50009:
                throw new InvalidOperationException("The staff already has an active meeting scheduled in the same time slot." + ex.Message);
            case 50010:
                throw new InvalidOperationException("The student already has an active meeting scheduled in the same time slot." + ex.Message);
            case 50011:
                throw new InvalidOperationException("The student has reached maximum pending meeting" + ex.Message);
            case 50012:
                throw new InvalidOperationException("The student has reached maximum canceled per 15 days" + ex.Message);
            case 547:
                throw new InvalidOperationException("Invalid meeting data. Ensure staff and student profiles exist." + ex.Message);

        }
        throw ex;
    }

    private bool IsValidAccess(BookedMeeting bookedMeeting, long roleId, long profileId)
    {
        if (roleId == (long)EUserRole.ADMIN) return true;
        if (roleId == (long)EUserRole.STUDENT) return bookedMeeting.StudentProfileId == profileId;
        if (roleId == (long)EUserRole.ADVISOR) return bookedMeeting.StaffProfileId == profileId;
        return false;
    }

    private TimeSpan GetTheTimeGap(DateTime time1, DateTime time2)
    => (time1 - time2).Duration();





    #endregion

    //get detail meeting
    public async Task<MeetingViewDetailResponse> GetDetailMeetingAsync(long meetingId, string accessToken)
    {
        var meeting = await _bookedMeetingRepository.GetDetailByIdAsync(meetingId);
        if (!IsValidAccess(meeting, _jWTService.GetRoleIdFromToken(accessToken), _jWTService.GetProfileIdFromToken(accessToken)))
            throw new InvalidAccessBookingAvailability("No permission to access this detail meeting");

        return _mapper.Map<MeetingViewDetailResponse>(meeting);
    }

}