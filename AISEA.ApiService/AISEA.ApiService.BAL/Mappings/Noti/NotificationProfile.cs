using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISEA.ApiService.SHARED.DTOs.Responses.Noti;
using AutoMapper;

namespace AISEA.ApiService.BAL.Mappings.Noti
{
    public class NotificationProfile: Profile
    {
        public NotificationProfile()
        {
            CreateMap<DAL.Entities.Notification, NotificationItemResponse>();
        }
    }
}