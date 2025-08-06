using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.DTOs.Requests.JoinedSubject;
using AISEA.ApiService.SHARED.DTOs.Responses.JoinedSubject;
using AISEA.ApiService.SHARED.Interfaces;
using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AISEA.ApiService.BAL.Services.CourseTracker;

public class JoinedSubjectService
{
    private readonly UserRepository _userRepository;
    private readonly JoinedSubjectRepository _joinedSubjectRepository;
    private readonly IMapper _mapper;
    private readonly IJWTService _jWTService;

    public async Task<JoinedSubjectStakeholderNotification> ImportSubjectAsync(SingleImportJoinedSubjectRequest request, string accessToken)
    {
        try
        {
            //get the student profile id from request.student user name
            var studentUser = await _userRepository.GetUserWStudentProfileAsync(request.StudentUserName);
            await _joinedSubjectRepository.CreateAsync(MapToJoinedSubject(request, studentUser.StudentProfile.Id, _jWTService.GetUsernameFromToken(accessToken)));

            return new JoinedSubjectStakeholderNotification
            {
                StakeholderUserId = studentUser.Id,
                Content = $"You have been successfully enrolled in the subject: {request.SubjectCode}.",
                Title = "Subject Enrollment Notification",
            };

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


    #region Private methods
    private void HandleMeetingSqlException(SqlException ex)
    {
        switch (ex.Number)
        {
            case 50013:
                throw new InvalidOperationException("Prerequisite not met");

            case 50015:
                throw new InvalidOperationException("Invalid subject code");

            case 50016:
                throw new InvalidOperationException("Invalid subject version code");

            case 50017:
                throw new InvalidOperationException("Invalid combo code");

            case 50018:
                throw new InvalidOperationException("Invalid curriculum code");

            case 50019:
                throw new InvalidOperationException("Invalid semester name, the semester name must be existed");
            case 50020:
                throw new InvalidOperationException("Student must have not graduated");

            case 547:
                throw new InvalidOperationException("Invalid joined subject data. Ensure student profile(s) exist.");

        }
        throw ex;
    }

    private JoinedSubject MapToJoinedSubject(SingleImportJoinedSubjectRequest request, long studentProfileId, string createdByUserName)
    {
        var joinedSubject = _mapper.Map<JoinedSubject>(request);
        joinedSubject.StudentProfileId = studentProfileId;
        joinedSubject.CreatedByUserName = createdByUserName;
        return joinedSubject;
    }




    #endregion
}