using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.Booking;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.DTOs.Responses.Booking;
using AISEA.ApiService.SHARED.Exceptions;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.SHARED.PropConfigs;
using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.BAL.Services.Booking;
//TODO: Replace Task<long> with MeetingNotiForPartnerResponse
public class BookedMeetingService
{
    //TODO: Temp reset the min day to confirm and min day to create = 2 for prod the cur = 0 for testing
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

    public async Task<long> AddReasonForOverdueAsync(string accessToken, ReasonOverdueRequest request)
    {
        //return the studentUserIdToNotify

        throw new NotImplementedException();
    }

    public async Task AdvCancelTheConfirmedAsync(long id, NoteDTO request, string accessToken)
    {
        throw new NotImplementedException();
    }

    public async Task StuCancelPendingAsync(long id, NoteDTO request, string accessToken)
    {
        throw new NotImplementedException();
        // //simply shift the status to the STU_CANCELED
        // var pendingMeeting = await _bookedMeetingRepository.GetByIdAsync(id);

        // if (pendingMeeting.Status != EBookingStatus.PENDING) throw new InvalidCurMeetingStatException("Cannot execute command on the meeting if the status of meeting is not current " + EBookingStatus.PENDING.ToString());//only this no need to check more about the time current w start time

        // if (!IsValidAccess(pendingMeeting, _jWTService.GetRoleIdFromToken(accessToken), _jWTService.GetProfileIdFromToken(accessToken))) throw new InvalidAccessMeeting("Deny permission");

        // pendingMeeting.Note = request.Note;
        // pendingMeeting.Status = EBookingStatus.STU_CANCELED;

        // await _bookedMeetingRepository.UpdateAsync(pendingMeeting);

    }

    public async Task<MeetingNotiForPartnerResponse> CompleteAsync(string accessToken, long id, InputCheckinRequest request)
    {

        //validate the time to do + current status + validate access + check in code (trigger)
        var completedMeeting = await _bookedMeetingRepository.GetByIdAsync(id);

        if (!IsValidAccess(completedMeeting, _jWTService.GetRoleIdFromToken(accessToken), _jWTService.GetProfileIdFromToken(accessToken))) throw new InvalidAccessBookingAvailability("Deny access to the meeting");


        if (!(completedMeeting.Status == EBookingStatus.CONFIRMED
        && DateTime.Now > completedMeeting.StartDateTime
        && request.CheckInCode == completedMeeting.CheckInCode
        )) throw new InvalidOperationException("Too soon to complete this meeting or the status/ checkin code of this meeting not true");

        completedMeeting.Status = EBookingStatus.COMPLETED;
        await _bookedMeetingRepository.UpdateAsync(completedMeeting);

        var studentProfile = await _studentProfileRepository.GetByIdAsync(completedMeeting.StudentProfileId);

        return new MeetingNotiForPartnerResponse
        {
            PartnerUserId = studentProfile.UserId,
            MeetingStartDateTime = completedMeeting.StartDateTime,
            MeetingEndDateTime = completedMeeting.EndDateTime,
            StatusChangedTo = EBookingStatus.COMPLETED
        };

    }

    public async Task<MeetingNotiForPartnerResponse> ConfirmMeetingAsync(long id, string accessToken)
    {
        //validate time + status + access permission
        var confirmedMeeting = await _bookedMeetingRepository.GetByIdAsync(id);

        if (!IsValidAccess(confirmedMeeting, _jWTService.GetRoleIdFromToken(accessToken), _jWTService.GetProfileIdFromToken(accessToken))) throw new InvalidAccessMeeting("You have no permission to confirm this meeting");

        var daysGap = GetTheTimeGap(confirmedMeeting.StartDateTime, DateTime.Now).TotalDays;

        if (!(confirmedMeeting.Status == EBookingStatus.PENDING
        && daysGap >= _bookingSettings.MinTimeAdvConfirmPendingMeetingDays
        )) throw new InvalidOperationException("Too late to confirm this meeting or the status of this meeting not true");

        confirmedMeeting.Status = EBookingStatus.CONFIRMED;
        await _bookedMeetingRepository.UpdateAsync(confirmedMeeting);

        var studentProfile = await _studentProfileRepository.GetByIdAsync(confirmedMeeting.StudentProfileId);

        return new MeetingNotiForPartnerResponse
        {
            PartnerUserId = studentProfile.UserId,
            MeetingStartDateTime = confirmedMeeting.StartDateTime,
            MeetingEndDateTime = confirmedMeeting.EndDateTime,
            StatusChangedTo = EBookingStatus.CONFIRMED
        };

    }

    public async Task<MeetingNotiForPartnerResponse> CreateMeetingAsync(CreateMeetingRequest request, string accessToken)
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
            var checkinCode = Guid.NewGuid().ToString();



            newMeeting.CheckInCode = checkinCode;

            //save into the database without caching
            await _bookedMeetingRepository.CreateAsync(newMeeting);

            var staffProfile = await _staffProfileRepository.GetByIdAsync(request.StaffProfileId);
            await _mailService.SendEmailAsync(_jWTService.GetEmailFromToken(accessToken), "CHECK IN CODE", $"The check in code for your meeting {request.StartDateTime} to {request.EndDateTime} is : {checkinCode}");
            //return the notified advisor user id
            return new MeetingNotiForPartnerResponse
            {
                PartnerUserId = staffProfile.UserId,
                MeetingStartDateTime = request.StartDateTime,
                MeetingEndDateTime = request.EndDateTime,
                StatusChangedTo = EBookingStatus.PENDING
            };
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
            {
                HandleLeaveSqlException(sqlEx);

            }
            throw;
        }
        catch (SqlException ex)
        {
            HandleLeaveSqlException(ex);
            throw;

        }

    }

    public async Task DeleteAsync(long id)
    {
        var removedMeeting = await _bookedMeetingRepository.GetByIdAsync(id);
        await _bookedMeetingRepository.RemoveAsync(removedMeeting);
    }

    public async Task<MeetingNotiForPartnerResponse> DisapprovePendingMeetingsAsync(string accessToken, DisApproveRequest request)
    {
        throw new NotImplementedException();

        //validate the access to meeting

        //shift to the status NOT_APPROVED with the same reason

        //have to check all whether before the start time and have status = PENDING ? (trigger)

        //save to the database

        //notify for all student related to the meeting(s) by the MeetingNotiForStudentResponses taken from trigger while saving into database

    }

    public async Task FeedbackAsync(string accessToken, FeedbackRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task GetAllAsync(PaginationRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<long> MarkAdvisorMissedAsync(string accessToken, long id, NoteDTO request)
    {
        //return the advisorUserIdToNotify
        throw new NotImplementedException();
    }

    public async Task<long> MarkStudentMissedAsync(string accessToken, long id)
    {
        //return the studentUserIdToNotify
        throw new NotImplementedException();
    }

    public async Task<int> StuCancelTheConfirmedAsync(long id, NoteDTO request, string accessToken)
    {
        throw new NotImplementedException();
    }



    #region Private methods
    private void HandleLeaveSqlException(SqlException ex)
    {
        switch (ex.Number)
        {
            case 50006:
                throw new InvalidOperationException("Student has reached the maximum number of bans (30). Cannot book meeting.");
            case 50007:
                throw new InvalidOperationException("The meeting time conflicts with staff's leave schedule.");
            case 50008:
                throw new InvalidOperationException("The meeting time does not exactly match staff's booking availability.");
            case 50009:
                throw new InvalidOperationException("The staff already has an active meeting scheduled in the same time slot.");
            case 50010:
                throw new InvalidOperationException("The student already has an active meeting scheduled in the same time slot.");
            case 547:
                throw new InvalidOperationException("Invalid meeting data. Ensure staff and student profiles exist.");

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
}