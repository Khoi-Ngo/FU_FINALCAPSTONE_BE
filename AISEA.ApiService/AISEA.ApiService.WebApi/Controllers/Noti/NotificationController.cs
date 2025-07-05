using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.Noti
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : BaseController
    {
        public NotificationController(EndpointSettings endpointSettings) : base(endpointSettings)
        {
        }
    }
}