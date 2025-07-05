using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;

namespace AISEA.ApiService.WebApi.Hubs
{
    //TODO
    public class NotificationHub : BaseHub
    {
        public NotificationHub(EndpointSettings endpointSettings) : base(endpointSettings)
        {
        }

        /// <summary>
        ///User get the pagination s of notification
        /// </summary>
        public async Task GetNotifications()
        {
            
        }
    }
}