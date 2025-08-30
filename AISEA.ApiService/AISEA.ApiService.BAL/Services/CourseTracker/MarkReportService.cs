using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.DTOs.Requests.MarkReport;
using AISEA.ApiService.SHARED.DTOs.Responses.MarkReport;
using AISEA.ApiService.SHARED.Exceptions;
using AISEA.ApiService.SHARED.Interfaces;
using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.BAL.Services.CourseTracker;

public class MarkReportService
{
    private readonly MarkReportRepository _markReportRepository;
    private readonly IJWTService _jWTService;
    private readonly IMapper _mapper;
    private readonly JoinedSubjectRepository _joinedSubjectRepository;

    public MarkReportService(MarkReportRepository markReportRepository, IJWTService jWTService, IMapper mapper, JoinedSubjectRepository joinedSubjectRepository)
    {
        _markReportRepository = markReportRepository;
        _jWTService = jWTService;
        _mapper = mapper;
        _joinedSubjectRepository = joinedSubjectRepository;
    }

    public async Task<long> DeleteAsync(long id)
    {
        var markReport = await _markReportRepository.GetByIdAsync(id);
        await _markReportRepository.RemoveAsync(markReport);
        return markReport.JoinedSubjectId;
    }


    public async Task<long> UpdateAsync(long id, CommandMarkRpRequest request)
    {
        var markReport = await _markReportRepository.GetByIdAsync(id);
        markReport = _mapper.Map(request, markReport);
        await _markReportRepository.UpdateAsync(markReport);
        return markReport.JoinedSubjectId;
    }

    public async Task ImportAsync(List<CommandMarkRpRequest> requests, long joinedSubjectID, string accessToken)
    {
        try
        {
            var importerEmail = _jWTService.GetEmailFromToken(accessToken);

            var subjectMarkReports = requests.Select(req =>
                MapToSubjectMarkReport(importerEmail, req, joinedSubjectID)).ToList();

            await _markReportRepository.CreateRangeAsync(subjectMarkReports);
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx)
            {
                HandleSqlException(sqlEx);
            }
            throw;
        }
        catch (SqlException ex)
        {
            HandleSqlException(ex);
            throw;
        }
    }


    private void HandleSqlException(SqlException ex)
    {
        if (ex.Number == 51021)
            throw new MarkReportWeightSumException("The total mark reports of joined subject cannot <0 and >100" + ex.Message);
    }


    public async Task<List<MarkReportResponse>> ViewByJoinedSubjectAsync(long joinedSubjectId)
    {
        var subjectMarkReports = await _markReportRepository.GetByJoinedSubjectAsync(joinedSubjectId);
        return _mapper.Map<List<MarkReportResponse>>(subjectMarkReports);
    }
    private SubjectMarkReport MapToSubjectMarkReport(string importerEmail, CommandMarkRpRequest request, long joinedSubjectID)
    {
        var subjectMarkReport = _mapper.Map<SubjectMarkReport>(request);
        subjectMarkReport.ScoreUpdatedBy = importerEmail;
        subjectMarkReport.JoinedSubjectId = joinedSubjectID;
        return subjectMarkReport;
    }

    public async Task UpdateStatusPassedAsync(long needCheckJoinedSubjectID)
    {
        var joinedSubject = await _joinedSubjectRepository
            .GetByIdWithCheckpointsAndPointsAsync(needCheckJoinedSubjectID);

        if (joinedSubject == null)
            throw new ArgumentException($"JoinedSubject with Id={needCheckJoinedSubjectID} not found.");

        var reports = joinedSubject.SubjectMarkReports;

        bool isPassed = true; // assume passed

        if (reports == null || !reports.Any())
        {
            // No reports means cannot pass
            isPassed = false;
        }
        else
        {
            foreach (var report in reports)
            {
                if (report == null) continue; // skip null entries safely

                if (report.Score < report.MinScore)
                {
                    isPassed = false;
                    break;
                }
            }
        }

        joinedSubject.IsPassed = isPassed;

        await _joinedSubjectRepository.UpdateAsync(joinedSubject);
    }

    public async Task<List<TranscriptItemResponse>> ViewPersonalTranscriptAsync(string accessToken)
    {
        var studentProfileId = _jWTService.GetProfileIdFromToken(accessToken);
        return await _joinedSubjectRepository.GetTranscriptAsync(studentProfileId);
    }

}