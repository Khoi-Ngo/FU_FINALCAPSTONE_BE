using System;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.SHARED.DTOs.Responses.AdvisorySession1to1;
using AISEA.ApiService.SHARED.DTOs.Responses.Message;
using AutoMapper;

namespace AISEA.ApiService.BAL.Mappings.Chat;

public class ChatProfile : Profile
{
    public ChatProfile()
    {
        CreateMap<AdvisorySession1to1, GetAdvisorySession1to1ItemsResponse>();
        CreateMap<Message, MessageItemResponse>();
    }
}