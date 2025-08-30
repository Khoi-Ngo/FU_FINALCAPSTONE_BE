using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.SHARED.DTOs.Requests.MarkReport;
using AISEA.ApiService.SHARED.DTOs.Responses.MarkReport;
using AutoMapper;

namespace AISEA.ApiService.BAL.Mappings.MarkReport;

public class MarkReportProfile : Profile
{
    public MarkReportProfile()
    {
        CreateMap<CommandMarkRpRequest, SubjectMarkReport>();
        CreateMap<SubjectMarkReport, MarkReportResponse>();
    }
}