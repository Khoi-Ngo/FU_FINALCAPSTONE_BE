using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.DTOs.Responses.Pagin;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories;

public class BookingAvailabilityRepository : GenericRepository<BookingAvailability>
{
    public BookingAvailabilityRepository(AiseaContext context) : base(context)
    {
    }

    public async Task BulkCreateAsync(List<BookingAvailability> bookingAvailabilities)
    {
        await _context.BookingAvailabilities.AddRangeAsync(bookingAvailabilities);
        await _context.SaveChangesAsync();
    }

    public async Task<HashSet<BookingAvailability>> GetAllByStaffProfileIdAsync(long staffProfileId)
    {
        return new HashSet<BookingAvailability>(await _context.BookingAvailabilities
            .Where(x => x.StaffProfileId == staffProfileId)
            .ToListAsync());
    }

    public async Task<PagedResult<BookingAvailability>> GetAllPagedAsync(PaginationRequest request)
    {
        var query = _context.BookingAvailabilities
            .Include(x => x.StaffProfile);
        var totalCount = await query.CountAsync();
        var bookingAvailabilities = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();
        return new PagedResult<BookingAvailability>
        {
            Items = bookingAvailabilities,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

}