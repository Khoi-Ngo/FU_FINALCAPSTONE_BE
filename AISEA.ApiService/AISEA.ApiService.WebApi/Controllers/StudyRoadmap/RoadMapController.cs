using AISEA.ApiService.BAL.Services.StudyRoadmap;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.StudyRoadmap
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoadMapController : BaseController
    {
        private readonly RoadmapService _roadmapService;
        public RoadMapController(EndpointSettings endpointSettings
        , RoadmapService roadmapService) : base(endpointSettings)
        {
            _roadmapService = roadmapService;
        }

        /// <summary>
        /// Create an Empty Roadmap for a student profile
        /// </summary>

        /// <summary>
        /// Delete a roadmap by roadmap id
        /// </summary>

        /// <summary>
        /// Create a single node in existed roadmap
        /// </summary>

        /// <summary>
        /// Remove all existed node in existed roadmap then Bulk create nodes in existed roadmap
        /// </summary>

        /// <summary>
        /// Delete a node in roadmap but keep other child nodes
        /// </summary>


        /// <summary>
        /// Get a node by node id
        /// </summary> 


        /// <summary>
        /// View roadmap with basic data in roadmap and all node related and applied data structure
        /// </summary>
    }
}