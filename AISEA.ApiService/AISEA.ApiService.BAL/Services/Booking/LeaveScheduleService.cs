using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.DTOs.Requests.Booking;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.DTOs.Responses.Booking;
using AISEA.ApiService.SHARED.DTOs.Responses.Pagin;
using AISEA.ApiService.SHARED.Exceptions;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.SHARED.PropConfigs;
using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.BAL.Services.Booking;

public class LeaveScheduleService
{
    #region Init
    private readonly LeaveScheduleRepository _leaveScheduleRepository;
    private readonly IMapper _mapper;
    private readonly IJWTService _jWTService;
    private readonly IHolidayService _holidayService;
    private readonly BookingSettings _bookingSettings;
    private readonly IRedisRepository _redisRepository;

    public LeaveScheduleService(LeaveScheduleRepository leaveScheduleRepository, IMapper mapper, IJWTService jWTService, IHolidayService holidayService, BookingSettings bookingSettings, IRedisRepository redisRepository)
    {
        _leaveScheduleRepository = leaveScheduleRepository;
        _mapper = mapper;
        _jWTService = jWTService;
        _holidayService = holidayService;
        _bookingSettings = bookingSettings;
        _redisRepository = redisRepository;
    }

    #endregion

    //create a leave schedule
    public async Task CreateAsync(CreateLeaveScheRequest request, string accessToken)
    {
        try
        {
            //check holiday
            var checkHoliday = await CheckHolidaysAsync(request.StartDateTime, request.EndDateTime);
            if (checkHoliday.Any()) throw new OnHolidayException("The register leaving includes holiday(s)", checkHoliday);

            var leaveSchedule = _mapper.Map<LeaveSchedule>(request);
            leaveSchedule.StaffProfileId = _jWTService.GetProfileIdFromToken(accessToken);

            await _leaveScheduleRepository.CreateAsync(leaveSchedule);
            //caching to the redis database
            await CacheLeaveScheAsync(leaveSchedule);
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
            {
                HandleLeaveSqlException(sqlEx);

            }
        }
        catch (SqlException ex)
        {
            HandleLeaveSqlException(ex);
        }
    }

    public async Task CreateBulkAsync(List<CreateLeaveScheRequest> requests, string accessToken)
    {
        try
        {
            var staffProfileId = _jWTService.GetProfileIdFromToken(accessToken);
            var leaveSchedules = new List<LeaveSchedule>();

            foreach (var request in requests)
            {
                // Validate holiday overlap for each request
                var holidays = await CheckHolidaysAsync(request.StartDateTime, request.EndDateTime);
                if (holidays.Any())
                {
                    throw new OnHolidayException($"Leave request from {request.StartDateTime} to {request.EndDateTime} includes holiday(s)", holidays);
                }

                var leaveSchedule = _mapper.Map<LeaveSchedule>(request);
                leaveSchedule.StaffProfileId = staffProfileId;
                leaveSchedules.Add(leaveSchedule);
            }

            // Use transaction for bulk insert
            using var transaction = await _leaveScheduleRepository.BeginTransactionAsync();
            try
            {
                await _leaveScheduleRepository.CreateBulkAsync(leaveSchedules);
                foreach (var schedule in leaveSchedules)
                {
                    CacheLeaveScheAsync(schedule);
                }
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx)
        {
            HandleLeaveSqlException(sqlEx);
        }
        catch (SqlException ex)
        {
            HandleLeaveSqlException(ex);
        }
    }


    //update a leave schedule
    public async Task UpdateAsync(UpdateLeaveScheRequest request, long id, string accessToken)
    {

        try
        {
            //check holiday
            var checkHoliday = await CheckHolidaysAsync(request.StartDateTime, request.EndDateTime);
            if (checkHoliday.Any()) throw new OnHolidayException("The register leaving includes holiday(s)", checkHoliday);

            var leaveSchedule = await GetLeaveScheduleAsync(id);
            if (!IsValidAccess(leaveSchedule, accessToken)) throw new InvalidAccessLeaveSche("Cannot access the Leave Schedule");

            _mapper.Map(request, leaveSchedule);
            await _leaveScheduleRepository.UpdateAsync(leaveSchedule);
            await CacheLeaveScheAsync(leaveSchedule);
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
            {
                HandleLeaveSqlException(sqlEx);

            }
        }
        catch (SqlException ex)
        {
            HandleLeaveSqlException(ex);
        }

    }

    //delete a leave schedule
    public async Task DeleteAsync(long id, string accessToken)
    {
        var leaveSchedule = await GetLeaveScheduleAsync(id);
        if (!IsValidAccess(leaveSchedule, accessToken)) throw new InvalidAccessLeaveSche("Cannot access the Leave Schedule");

        await _leaveScheduleRepository.RemoveAsync(leaveSchedule);
        var cachedKey = $"{_bookingSettings.LeaveSchePrefix}{id}";
        await _redisRepository.RemoveByKeyAsync(cachedKey);
    }


    public async Task<PagedResult<LeaveScheListSimplyResponse>> GetAllSimplyAsync(PaginationRequest request)
    {
        var (leaveSchedules, totalCount) = await _leaveScheduleRepository.GetAllAsync(request);
        return new PagedResult<LeaveScheListSimplyResponse>
        {
            Items = _mapper.Map<List<LeaveScheListSimplyResponse>>(leaveSchedules),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<PagedResult<LeaveScheListSimplyResponse>> GetAllSimplyAsync(PaginationRequest request, long staffProfileId)
    {
        var (leaveSchedules, totalCount) = await _leaveScheduleRepository.GetAllByStaffProfileIdAsync(request, staffProfileId);
        return new PagedResult<LeaveScheListSimplyResponse>
        {
            Items = _mapper.Map<List<LeaveScheListSimplyResponse>>(leaveSchedules),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<PagedResult<LeaveScheListSimplyResponse>> GetAllSimplyAsync(PaginationRequest request, string accessToken)
    => await GetAllSimplyAsync(request, _jWTService.GetProfileIdFromToken(accessToken));

    //get single by id
    public async Task<LeaveScheListSimplyResponse> GetSimplyByIdAsync(long id)
    {
        var bookingAvailability = await GetLeaveScheduleAsync(id);
        return _mapper.Map<LeaveScheListSimplyResponse>(bookingAvailability);
    }



    #region private methods


    //get array of dateonly from DateTime Start - DateTime End
    private List<DateOnly> GetDateOnliesFromDateTimeRange(DateTime start, DateTime end) =>
     Enumerable.Range(0, (end.Date - start.Date).Days + 1)
               .Select(offset => DateOnly.FromDateTime(start.Date.AddDays(offset)))
               .ToList();

    //check holidays
    private async Task<List<HolidayResponse>> CheckHolidaysAsync(DateTime start, DateTime end)
    {
        var leavingDates = GetDateOnliesFromDateTimeRange(start, end);

        // Create a list of tasks to run in parallel
        var holidayTasks = leavingDates
            .Select(date => _holidayService.CheckHolidayAsync(date))
            .ToList();

        // Wait for all tasks to complete
        var holidayResults = await Task.WhenAll(holidayTasks);

        // Combine results into a single list
        return holidayResults.SelectMany(r => r).ToList();
    }



    private void HandleLeaveSqlException(SqlException ex)
    {
        switch (ex.Number)
        {
            case 2601: // Unique index violation
            case 2627: // Unique constraint violation
                throw new LeaveScheduleDuplicateEx("A leave schedule with the same start time, end time, and staff profile already exists.");
            case 50003: // Overlap
                throw new LeaveScheduleOverlapEx("The leave schedule overlaps with an existing leave schedule for the same staff.");
            case 50004: // No matching booking availability
                throw new NoMatchingBookingAvailabilityEx("No matching booking availability found for the specified staff and time range.");
            case 50005: // Existing Active meetings
                throw new LeaveScheduleConflictWithMeetingsEx("Cannot register leave due to existing Active meetings. Cancel/Disapprove those meetings first.");
            case 547:
                throw new InvalidOperationException("Invalid leave schedule data. Ensure staff profile exists.");
            default:
                throw new InvalidOperationException("An error occurred while processing the leave schedule.", ex);
        }
    }

    private async Task CacheLeaveScheAsync(LeaveSchedule leaveSchedule)
    {
        var cachedKey = $"{_bookingSettings.LeaveSchePrefix}{leaveSchedule.Id}";
        await _redisRepository.SetValueAsync<LeaveSchedule>(
            cachedKey,
            leaveSchedule,
            TimeSpan.FromDays(_bookingSettings.ExpiredLeaveScheDaysCached));
    }

    private async Task<LeaveSchedule> GetLeaveScheduleAsync(long id)
    {
        var cachedKey = $"{_bookingSettings.LeaveSchePrefix}{id}";
        var leaveSche = await _redisRepository.GetValueAsync<LeaveSchedule>(cachedKey);

        if (leaveSche is null)
        {
            leaveSche = await _leaveScheduleRepository.GetByIdAsync(id);
            if (leaveSche is null)
                throw new NotFoundException("There is no leave schedule with the specified ID");

            await _redisRepository.SetValueAsync<LeaveSchedule>(
                cachedKey,
                leaveSche,
                TimeSpan.FromDays(_bookingSettings.ExpiredLeaveScheDaysCached));
        }

        return leaveSche;
    }

    private bool IsValidAccess(LeaveSchedule leaveSchedule, string accessToken)
    {
        var profileId = _jWTService.GetProfileIdFromToken(accessToken);
        return leaveSchedule.StaffProfileId == profileId;
    }

    public async Task<DateTime> CheckDateTimeDBAsync()
    {
        return await _leaveScheduleRepository.GetDatabaseUtcDateTimeAsync();
    }

    public async Task<object> CheckDayOfWeekSQLAsync(DateTime date)
    {
        return await _leaveScheduleRepository.CheckDayOfWeekSQLAsync(date);
    }


    #endregion

}