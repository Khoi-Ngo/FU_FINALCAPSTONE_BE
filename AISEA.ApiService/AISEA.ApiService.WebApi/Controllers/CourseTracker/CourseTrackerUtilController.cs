using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.CourseTracker
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseTrackerUtilController : BaseController
    {
        public CourseTrackerUtilController(EndpointSettings endpointSettings) : base(endpointSettings)
        {
        }
    }
}