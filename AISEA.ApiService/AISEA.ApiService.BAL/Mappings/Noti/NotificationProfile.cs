using AISEA.ApiService.SHARED.DTOs.Requests.Noti;
using AISEA.ApiService.SHARED.DTOs.Responses.Noti;
using AutoMapper;

namespace AISEA.ApiService.BAL.Mappings.Noti
{
    public class NotificationProfile: Profile
    {
        public NotificationProfile()
        {
            CreateMap<DAL.Entities.Notification, NotificationItemResponse>();

            CreateMap<NotificationDTO, DAL.Entities.Notification>();
        }
    }
}