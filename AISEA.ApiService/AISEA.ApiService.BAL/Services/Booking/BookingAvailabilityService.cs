using System.Security.Authentication;
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

namespace AISEA.ApiService.BAL.Services.Booking;

public class BookingAvailabilityService
{
    private readonly BookingAvailabilityRepository _bookingAvailabilityRepository;
    private readonly IJWTService _jwtService;
    private readonly IMapper _mapper;
    private readonly IRedisRepository _redisRepository;
    private readonly BookingSettings _bookingSettings;

    public BookingAvailabilityService(BookingAvailabilityRepository bookingAvailabilityRepository, IJWTService jwtService, IMapper mapper, IRedisRepository redisRepository, BookingSettings bookingSettings)
    {
        _bookingAvailabilityRepository = bookingAvailabilityRepository;
        _jwtService = jwtService;
        _mapper = mapper;
        _redisRepository = redisRepository;
        _bookingSettings = bookingSettings;
    }


    //bulk create booking availability for a staff
    public async Task<List<BookingAvailability>> BulkCreateBookingAvailabilityAsync(List<CreateBookingAvailabilityRequest> request, string accessToken)
    {
        //get profile id from access token
        var staffProfileId = _jwtService.GetProfileIdFromToken(accessToken);

        if (staffProfileId == 0) throw new InvalidCredentialException("Staff profile ID is required.");

        //create booking availability entities
        var bookingAvailabilities = _mapper.Map<List<BookingAvailability>>(request);
        foreach (var availability in bookingAvailabilities)
        {
            availability.StaffProfileId = staffProfileId;
        }

        await _bookingAvailabilityRepository.BulkCreateAsync(bookingAvailabilities);
        // Cache each booking availability
        await CacheBookingAvailabilitiesAsync(bookingAvailabilities);
        return bookingAvailabilities;
    }


    //create booking availability for a staff
    public async Task<BookingAvailability> CreateBookingAvailabilityAsync(CreateBookingAvailabilityRequest request, string accessToken)
    {
        //get profile id from access token
        var staffProfileId = _jwtService.GetProfileIdFromToken(accessToken);

        if (staffProfileId == 0) throw new InvalidCredentialException("Staff profile ID is required.");

        //create booking availability entity
        var bookingAvailability = _mapper.Map<BookingAvailability>(request);
        bookingAvailability.StaffProfileId = staffProfileId;

        await _bookingAvailabilityRepository.CreateAsync(bookingAvailability);

        await CacheBookingAvailabilityAsync(bookingAvailability);

        return bookingAvailability;
    }


    //get all booking availability for a staff
    public async Task<HashSet<BookingAvailability>> GetBookingAvailabilitiesAsync(long staffProfileId)
    {
        return await _bookingAvailabilityRepository.GetAllByStaffProfileIdAsync(staffProfileId);
    }

    //get all booking availability pagination
    public async Task<PagedResult<BookingAvailabilityListItemResponse>> GetAllPagedAsync(PaginationRequest request)
    {
        //get all booking availabilities from database
        var (bookingAvailabilities, totalCount) = await _bookingAvailabilityRepository.GetAllPagedAsync(request);

        return new PagedResult<BookingAvailabilityListItemResponse>
        {
            Items = _mapper.Map<List<BookingAvailabilityListItemResponse>>(bookingAvailabilities),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    //edit booking availability
    public async Task<BookingAvailability> UpdateAsync(long id, UpdateBookingAvailabilityRequest request, string accessToken)
    {
        //get the booking availability
        var bookingAvailability = await GetBookingAvailabilityAsync(id);

        if (!IsValidAccess(bookingAvailability, accessToken)) throw new InvalidAccessBookingAvailability("Cannot access the booking availability");

        bookingAvailability = _mapper.Map<BookingAvailability>(request);
        //save DB
        await _bookingAvailabilityRepository.UpdateAsync(bookingAvailability);
        //caching
        await CacheBookingAvailabilityAsync(bookingAvailability);

        return bookingAvailability;
    }

    //delete booking availability
    public async Task DeleteAsync(long id, string accessToken)
    {
        var bookingAvailability = await GetBookingAvailabilityAsync(id);
        if (!IsValidAccess(bookingAvailability, accessToken)) throw new InvalidAccessBookingAvailability("No authorize to delete");
        await _bookingAvailabilityRepository.RemoveAsync(bookingAvailability);
    }


    #region caching with redis database
    private async Task CacheBookingAvailabilityAsync(BookingAvailability bookingAvailability)
    {
        //try to get the cached booking availability by staffProfileId
        var cachedKey = $"{_bookingSettings.BookingAvaiPrefix}{bookingAvailability.Id}";
        await _redisRepository.SetValueAsync<BookingAvailability>(cachedKey, bookingAvailability, TimeSpan.FromDays(_bookingSettings.ExpiredBookingAvaiDays));
    }
    private async Task<BookingAvailability> GetBookingAvailabilityAsync(long bookingAvailabilityId)
    {
        var cachedKey = $"{_bookingSettings.BookingAvaiPrefix}{bookingAvailabilityId}";
        var bookingAvailability = await _redisRepository.GetValueAsync<BookingAvailability>(cachedKey);

        if (bookingAvailability is null)
        {
            //query from database
            bookingAvailability = await _bookingAvailabilityRepository.GetByIdAsync(bookingAvailabilityId);

            //caching into the redis
            await _redisRepository.SetValueAsync<BookingAvailability>(cachedKey, bookingAvailability, TimeSpan.FromDays(_bookingSettings.ExpiredBookingAvaiDays));
        }

        if (bookingAvailability is null) throw new NotFoundException("There is no booking availability id");
        return bookingAvailability;
    }

    private async Task CacheBookingAvailabilitiesAsync(List<BookingAvailability> bookingAvailabilities)
    {
        var cacheTasks = bookingAvailabilities.Select(availability =>
        {
            var cachedKey = $"{_bookingSettings.BookingAvaiPrefix}{availability.Id}";
            return _redisRepository.SetValueAsync<BookingAvailability>(cachedKey, availability, TimeSpan.FromDays(_bookingSettings.ExpiredBookingAvaiDays));
        });
        await Task.WhenAll(cacheTasks);
    }
    #endregion


    private bool IsValidAccess(BookingAvailability bookingAvailability, string accessToken)
    {
        var profileId = _jwtService.GetProfileIdFromToken(accessToken);
        return bookingAvailability.StaffProfileId == profileId;
    }



}