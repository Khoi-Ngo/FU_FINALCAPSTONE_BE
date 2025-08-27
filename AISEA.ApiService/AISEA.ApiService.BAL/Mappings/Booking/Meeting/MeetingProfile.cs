using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.SHARED.DTOs.Requests.Booking;
using AISEA.ApiService.SHARED.DTOs.Responses.Booking;
using AutoMapper;

namespace AISEA.ApiService.BAL.Mappings.Booking.Meeting;

public class MeetingProfile : Profile
{
    public MeetingProfile()
    {
        // Map Create Request -> Entity
        CreateMap<CreateMeetingRequest, BookedMeeting>();

        // Map BookedMeeting -> MeetingViewDetailResponse
        CreateMap<BookedMeeting, MeetingViewDetailResponse>()
            .ForMember(dest => dest.StaffProfileId, opt => opt.MapFrom(src => src.StaffProfileId))
            .ForMember(dest => dest.StaffCampus, opt => opt.MapFrom(src => src.StaffProfile.Campus))
            .ForMember(dest => dest.StaffDepartment, opt => opt.MapFrom(src => src.StaffProfile.Department))
            .ForMember(dest => dest.StaffPosition, opt => opt.MapFrom(src => src.StaffProfile.Position))
            .ForMember(dest => dest.StaffStartWorkAt, opt => opt.MapFrom(src => src.StaffProfile.StartWorkAt))
            .ForMember(dest => dest.StaffEndWorkAt, opt => opt.MapFrom(src => src.StaffProfile.EndWorkAt))
            .ForMember(dest => dest.StaffUserId, opt => opt.MapFrom(src => src.StaffProfile.UserId))
            .ForMember(dest => dest.StaffUsername, opt => opt.MapFrom(src => src.StaffProfile.User.Username))
            .ForMember(dest => dest.StaffEmail, opt => opt.MapFrom(src => src.StaffProfile.User.Email))
            .ForMember(dest => dest.StaffFirstName, opt => opt.MapFrom(src => src.StaffProfile.User.FirstName))
            .ForMember(dest => dest.StaffLastName, opt => opt.MapFrom(src => src.StaffProfile.User.LastName))
            .ForMember(dest => dest.StaffDateOfBirth, opt => opt.MapFrom(src => src.StaffProfile.User.DateOfBirth))
            .ForMember(dest => dest.StaffAvatarUrl, opt => opt.MapFrom(src => src.StaffProfile.User.AvatarUrl))
            .ForMember(dest => dest.StaffStatus, opt => opt.MapFrom(src => src.StaffProfile.User.Status))
            .ForMember(dest => dest.StudentProfileId, opt => opt.MapFrom(src => src.StudentProfileId))
            .ForMember(dest => dest.StudentEnrolledAt, opt => opt.MapFrom(src => src.StudentProfile.EnrolledAt))
            .ForMember(dest => dest.StudentNumberOfBan, opt => opt.MapFrom(src => src.StudentProfile.NumberOfBan))
            .ForMember(dest => dest.StudentCareerGoal, opt => opt.MapFrom(src => src.StudentProfile.CareerGoal))
            .ForMember(dest => dest.StudentProgramId, opt => opt.MapFrom(src => src.StudentProfile.ProgramId))
            .ForMember(dest => dest.StudentUserId, opt => opt.MapFrom(src => src.StudentProfile.UserId))
            .ForMember(dest => dest.StudentUsername, opt => opt.MapFrom(src => src.StudentProfile.User.Username))
            .ForMember(dest => dest.StudentEmail, opt => opt.MapFrom(src => src.StudentProfile.User.Email))
            .ForMember(dest => dest.StudentFirstName, opt => opt.MapFrom(src => src.StudentProfile.User.FirstName))
            .ForMember(dest => dest.StudentLastName, opt => opt.MapFrom(src => src.StudentProfile.User.LastName))
            .ForMember(dest => dest.StudentDateOfBirth, opt => opt.MapFrom(src => src.StudentProfile.User.DateOfBirth))
            .ForMember(dest => dest.StudentAvatarUrl, opt => opt.MapFrom(src => src.StudentProfile.User.AvatarUrl))
            .ForMember(dest => dest.StudentStatus, opt => opt.MapFrom(src => src.StudentProfile.User.Status));

        // Map BookedMeeting -> MeetingItemListResponse
        CreateMap<BookedMeeting, MeetingItemListResponse>()
            .ForMember(dest => dest.StaffProfileId, opt => opt.MapFrom(src => src.StaffProfileId))
            .ForMember(dest => dest.StaffFirstName, opt => opt.MapFrom(src => src.StaffProfile.User.FirstName))
            .ForMember(dest => dest.StaffLastName, opt => opt.MapFrom(src => src.StaffProfile.User.LastName))
            .ForMember(dest => dest.StaffEmail, opt => opt.MapFrom(src => src.StaffProfile.User.Email))
            .ForMember(dest => dest.StudentProfileId, opt => opt.MapFrom(src => src.StudentProfileId))
            .ForMember(dest => dest.StudentFirstName, opt => opt.MapFrom(src => src.StudentProfile.User.FirstName))
            .ForMember(dest => dest.StudentLastName, opt => opt.MapFrom(src => src.StudentProfile.User.LastName))
            .ForMember(dest => dest.StudentEmail, opt => opt.MapFrom(src => src.StudentProfile.User.Email));
    }
}