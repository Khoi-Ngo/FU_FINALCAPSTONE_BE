using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.SHARED.DTOs.Responses.AdvisorySession1to1;
using AutoMapper;

namespace AISEA.ApiService.BAL.Mappings.Chat
{
    public class ChatProfile : Profile
    {
        public ChatProfile()
        {
            CreateMap<AdvisorySession1to1, GetAdvisorySession1to1ListResponse>();

            CreateMap<Message, MessageDataListResponse>()
                .ForMember(dest => dest.SenderUserName, opt => opt.MapFrom(src => src.Sender.Username))
                ;

            CreateMap<AdvisorySession1to1, GetAdvisorySession1to1DetailResponse>()
                .ForMember(dest => dest.MessagesDataList, opt => opt.MapFrom(src => src.Messages));

        }
    }
}