using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.SHARED.DTOs.Requests.Program;
using AISEA.ApiService.SHARED.DTOs.Responses.Program;
using AutoMapper;

namespace AISEA.ApiService.BAL.Mappings.Program
{
    public class ProgramProfile : Profile
    {
        public ProgramProfile()
        {
            CreateMap<CreateProgramRequest, DAL.Entities.Program>();
            CreateMap<UpdateProgramRequest, DAL.Entities.Program>();
            CreateMap<DAL.Entities.Program, GetProgramResponse>();
        }
    }
}