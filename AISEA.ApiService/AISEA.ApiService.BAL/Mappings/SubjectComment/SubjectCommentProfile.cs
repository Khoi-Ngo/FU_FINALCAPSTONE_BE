using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.SHARED.DTOs.Requests.SubjectComment;
using AISEA.ApiService.SHARED.DTOs.Responses.SubjectComment;
using AutoMapper;

namespace AISEA.ApiService.BAL.Mappings.SubjectComment
{
    public class SubjectCommentProfile : Profile
    {
        public SubjectCommentProfile()
        {
            CreateMap<CreateSubjectCommentRequest, DAL.Entities.SubjectComment>();
            CreateMap<DAL.Entities.SubjectComment, SubjectCommentResponse>();
        }
    }
}
