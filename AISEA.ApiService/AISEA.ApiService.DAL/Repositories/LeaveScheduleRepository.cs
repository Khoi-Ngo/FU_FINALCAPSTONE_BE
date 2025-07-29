using AISEA.ApiService.DAL.Abstract;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Persistence;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.DTOs.Responses.Pagin;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.DAL.Repositories;

public class LeaveScheduleRepository : GenericRepository<LeaveSchedule>
{
    public LeaveScheduleRepository(AiseaContext context) : base(context)
    {
    }

    public async Task<object> CheckDayOfWeekSQLAsync(DateTime date)
    {
        using (var command = _context.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = @"
            SELECT 
                @InputDate AS InputDate,
                DATEPART(WEEKDAY, @InputDate) AS DayOfWeekNumber,
                DATENAME(WEEKDAY, @InputDate) AS DayOfWeekName
        ";
            command.CommandType = System.Data.CommandType.Text;

            var param = command.CreateParameter();
            param.ParameterName = "@InputDate";
            param.Value = date;
            command.Parameters.Add(param);

            await _context.Database.OpenConnectionAsync();

            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return new
                    {
                        InputDate = reader.GetDateTime(0),
                        DayOfWeekNumber = reader.GetInt32(1),
                        DayOfWeekName = reader.GetString(2)
                    };
                }
            }

            return new { Error = "No result returned" };
        }
    }


    public async Task<(IEnumerable<LeaveSchedule> leaveSchedules, int TotalCount)> GetAllAsync(PaginationRequest request)
    {
        var query = _context.LeaveSchedules.OrderByDescending(l => l.StartDateTime);
        var totalCount = await query.CountAsync();
        var leaveSchedules = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();
        return (leaveSchedules, totalCount);
    }

    public async Task<(IEnumerable<LeaveSchedule> leaveSchedules, int TotalCount)> GetAllByStaffProfileIdAsync(PaginationRequest request, long staffProfileId)
    {
        var query = _context.LeaveSchedules.Where(l => l.StaffProfileId == staffProfileId).OrderByDescending(l => l.StartDateTime);
        var totalCount = await query.CountAsync();
        var leaveSchedules = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();
        return (leaveSchedules, totalCount);
    }



    public async Task<DateTime> GetDatabaseUtcDateTimeAsync()
    {
        using var command = _context.Database.GetDbConnection().CreateCommand();
        command.CommandText = "SELECT GETUTCDATE()";
        _context.Database.OpenConnection();

        var result = await command.ExecuteScalarAsync();
        return (DateTime)result;
    }

}