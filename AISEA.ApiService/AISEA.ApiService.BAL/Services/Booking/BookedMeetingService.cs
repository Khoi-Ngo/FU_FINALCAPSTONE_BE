using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.DTOs.Requests.Booking;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.BAL.Services.Booking;

public class BookedMeetingService
{
    private readonly BookedMeetingRepository _bookedMeetingRepository;
    private readonly IJWTService _jWTService;
    private readonly IRedisRepository _redisRepository;
    private readonly IHolidayService _holidayService;

    public async Task<long> AddReasonForOverdueAsync(string accessToken, ReasonOverdueRequest request)
    {
        //return the studentUserIdToNotify

        throw new NotImplementedException();
    }

    public async Task AdvCancelTheConfirmedAsync(long id, NoteDTO request, string accessToken)
    {
        throw new NotImplementedException();
    }

    public async Task CancelPendingAsync(long id, NoteDTO request, string accessToken)
    {
        throw new NotImplementedException();
    }

    public async Task CompleteAsync(string accessToken, long id, InputCheckinRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task ConfirmMeetingAsync(long id, string accessToken)
    {
        throw new NotImplementedException();
    }

    public async Task CreateMeetingAsync(CreateMeetingRequest request, string accessToken)
    {
        throw new NotImplementedException();
        // try
        // {
        //     //avoid create meeting at holiday
        //     if()
        // }
        // catch (DbUpdateException ex)
        // {
        //     if (ex.InnerException is SqlException sqlEx)
        //     {
        //         HandleLeaveSqlException(sqlEx);

        //     }
        // }
        // catch (SqlException ex)
        // {
        //     HandleLeaveSqlException(ex);

        // }

    }

    public async Task DeleteAsync(long id)
    {
        throw new NotImplementedException();
    }

    public async Task DisapprovePendingMeetingsAsync(string accessToken, DisApproveRequest request)
    {
        throw new NotImplementedException();
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
        // switch (ex.Number)
        // {
       
        // }
    }
    #endregion
}