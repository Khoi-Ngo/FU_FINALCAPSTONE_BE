using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.SHARED.DTOs.Requests.Subject;
using AISEA.ApiService.SHARED.DTOs.Responses.Subject;
using AutoMapper;

namespace AISEA.ApiService.BAL.Mappings.Subject
{
    public class SubjectProfile : Profile
    {
        public SubjectProfile()
        {
            CreateMap<CreateSubjectRequest, DAL.Entities.Subject>();
            CreateMap<UpdateSubjectRequest, DAL.Entities.Subject>();
            CreateMap<DAL.Entities.Subject, GetSubjectResponse>();
        }
    }
}