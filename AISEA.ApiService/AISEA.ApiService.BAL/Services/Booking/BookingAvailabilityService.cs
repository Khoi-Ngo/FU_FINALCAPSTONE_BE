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
using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace AISEA.ApiService.BAL.Services.Booking;

public class BookingAvailabilityService
{
    private readonly BookingAvailabilityRepository _bookingAvailabilityRepository;
    private readonly IJWTService _jwtService;
    private readonly IMapper _mapper;
    private readonly IRedisRepository _redisRepository;
    private readonly BookingSettings _bookingSettings;

    public BookingAvailabilityService(
        BookingAvailabilityRepository bookingAvailabilityRepository,
        IJWTService jwtService,
        IMapper mapper,
        IRedisRepository redisRepository,
        BookingSettings bookingSettings)
    {
        _bookingAvailabilityRepository = bookingAvailabilityRepository;
        _jwtService = jwtService;
        _mapper = mapper;
        _redisRepository = redisRepository;
        _bookingSettings = bookingSettings;
    }

    // Bulk create booking availability for a staff
    public async Task BulkCreateBookingAvailabilityAsync(List<CreateBookingAvailabilityRequest> request, string accessToken)
    {
        var staffProfileId = _jwtService.GetProfileIdFromToken(accessToken);
        if (staffProfileId == 0) throw new InvalidCredentialException("Staff profile ID is required.");

        var bookingAvailabilities = _mapper.Map<List<BookingAvailability>>(request);
        foreach (var availability in bookingAvailabilities)
        {
            availability.StaffProfileId = staffProfileId;
        }

        try
        {
            await _bookingAvailabilityRepository.BulkCreateAsync(bookingAvailabilities);
            await CacheBookingAvailabilitiesAsync(bookingAvailabilities);
        }
        catch (SqlException ex)
        {
            HandleSqlException(ex);
            throw; // Re-throw if not handled
        }
    }

    // Create booking availability for a staff
    public async Task CreateBookingAvailabilityAsync(CreateBookingAvailabilityRequest request, string accessToken)
    {
        var staffProfileId = _jwtService.GetProfileIdFromToken(accessToken);
        if (staffProfileId == 0) throw new InvalidCredentialException("Staff profile ID is required.");

        var bookingAvailability = _mapper.Map<BookingAvailability>(request);
        bookingAvailability.StaffProfileId = staffProfileId;

        try
        {
            await _bookingAvailabilityRepository.CreateAsync(bookingAvailability);
            await CacheBookingAvailabilityAsync(bookingAvailability);
        }
        catch (SqlException ex)
        {
            HandleSqlException(ex);
            throw;
        }
    }

    // Get all booking availability for a staff
    public async Task<HashSet<BookingAvailability>> GetBookingAvailabilitiesAsync(long staffProfileId)
    {
        return await _bookingAvailabilityRepository.GetAllByStaffProfileIdAsync(staffProfileId);
    }

    // Get all booking availability with pagination
    public async Task<PagedResult<BookingAvailabilityListItemResponse>> GetAllPagedAsync(PaginationRequest request)
    {
        var (bookingAvailabilities, totalCount) = await _bookingAvailabilityRepository.GetAllPagedAsync(request);
        return new PagedResult<BookingAvailabilityListItemResponse>
        {
            Items = _mapper.Map<List<BookingAvailabilityListItemResponse>>(bookingAvailabilities),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    // Edit booking availability
    public async Task UpdateAsync(long id, UpdateBookingAvailabilityRequest request, string accessToken)
    {
        var bookingAvailability = await GetBookingAvailabilityAsync(id);
        if (!IsValidAccess(bookingAvailability, accessToken))
            throw new InvalidAccessBookingAvailability("Cannot access the booking availability");

        // Map updated fields, preserving Id and StaffProfileId
        _mapper.Map(request, bookingAvailability);

        try
        {
            await _bookingAvailabilityRepository.UpdateAsync(bookingAvailability);
            await CacheBookingAvailabilityAsync(bookingAvailability);
        }
        catch (SqlException ex)
        {
            HandleSqlException(ex);
            throw;
        }
    }

    // Delete booking availability
    public async Task DeleteAsync(long id, string accessToken)
    {
        var bookingAvailability = await GetBookingAvailabilityAsync(id);
        if (!IsValidAccess(bookingAvailability, accessToken))
            throw new InvalidAccessBookingAvailability("No authorization to delete");

        await _bookingAvailabilityRepository.RemoveAsync(bookingAvailability);
        // Optionally, remove from cache
        var cachedKey = $"{_bookingSettings.BookingAvaiPrefix}{id}";
        await _redisRepository.RemoveByKeyAsync(cachedKey);
    }

    #region Private Methods

    private async Task CacheBookingAvailabilityAsync(BookingAvailability bookingAvailability)
    {
        var cachedKey = $"{_bookingSettings.BookingAvaiPrefix}{bookingAvailability.Id}";
        await _redisRepository.SetValueAsync<BookingAvailability>(
            cachedKey,
            bookingAvailability,
            TimeSpan.FromDays(_bookingSettings.ExpiredBookingAvaiDays));
    }

    private async Task<BookingAvailability> GetBookingAvailabilityAsync(long bookingAvailabilityId)
    {
        var cachedKey = $"{_bookingSettings.BookingAvaiPrefix}{bookingAvailabilityId}";
        var bookingAvailability = await _redisRepository.GetValueAsync<BookingAvailability>(cachedKey);

        if (bookingAvailability is null)
        {
            bookingAvailability = await _bookingAvailabilityRepository.GetByIdAsync(bookingAvailabilityId);
            if (bookingAvailability is null)
                throw new NotFoundException("There is no booking availability with the specified ID");

            await _redisRepository.SetValueAsync<BookingAvailability>(
                cachedKey,
                bookingAvailability,
                TimeSpan.FromDays(_bookingSettings.ExpiredBookingAvaiDays));
        }

        return bookingAvailability;
    }

    private async Task CacheBookingAvailabilitiesAsync(List<BookingAvailability> bookingAvailabilities)
    {
        var cacheTasks = bookingAvailabilities.Select(availability =>
        {
            var cachedKey = $"{_bookingSettings.BookingAvaiPrefix}{availability.Id}";
            return _redisRepository.SetValueAsync<BookingAvailability>(
                cachedKey,
                availability,
                TimeSpan.FromDays(_bookingSettings.ExpiredBookingAvaiDays));
        });
        await Task.WhenAll(cacheTasks);
    }

    private bool IsValidAccess(BookingAvailability bookingAvailability, string accessToken)
    {
        var profileId = _jwtService.GetProfileIdFromToken(accessToken);
        return bookingAvailability.StaffProfileId == profileId;
    }

    private void HandleSqlException(SqlException ex)
    {
        if (ex.Number == 50001) // Trigger error for overlap
            throw new ValidationException("The time slot overlaps with an existing slot for the same staff and day.");
        if (ex.Number == 2601 || ex.Number == 2627) // Unique constraint violation
            throw new ValidationException("A time slot with the same start time, end time, day, and staff already exists.");
    }

    #endregion
}