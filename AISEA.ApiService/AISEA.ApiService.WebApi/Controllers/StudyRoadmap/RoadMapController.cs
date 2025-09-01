using AISEA.ApiService.BAL.Services.StudyRoadmap;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.Filters;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using AISEA.ApiService.WebApi.InterceptorAPI;
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
        [HttpPost("create/{studentId}")]
        [AuditLog(Tag = "CREATE_EMPTY_STUDY_ROADMAP")]
        [PermissionAuthorize((int)EUserRole.STUDENT)]
        public async Task<IActionResult> CreateRoadmap(long studentId, [FromQuery] string name)
        {
            var roadmap = await _roadmapService.CreateRoadmapAsync(studentId, name);
            return Ok(roadmap);
        }

        /// <summary>
        /// Delete a roadmap by roadmap id
        /// </summary>
        [HttpDelete("{roadmapId}")]
        [AuditLog(Tag = "DELETE_STUDY_ROADMAP")]
        [PermissionAuthorize((int)EUserRole.STUDENT)]
        public async Task<IActionResult> DeleteRoadmap(long roadmapId)
        {
            var result = await _roadmapService.DeleteRoadmapAsync(roadmapId);
            if (!result) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Create a single node in existed roadmap
        /// </summary>
        [HttpPost("{roadmapId}/node")]
        [AuditLog(Tag = "CREATE_SINGLE_NODE_STUDY_ROADMAP")]
        [PermissionAuthorize((int)EUserRole.STUDENT)]
        public async Task<IActionResult> CreateNode(long roadmapId, [FromBody] CreateNodeDto node)
        {
            var created = await _roadmapService.CreateNodeAsync(roadmapId, node);
            return Ok(created);
        }



        /// <summary>
        /// Delete a node in roadmap but keep other child nodes
        /// </summary>
        [HttpDelete("node/{nodeId}")]
        [AuditLog(Tag = "DELETE_SINGLE_NODE_STUDY_ROADMAP")]
        [PermissionAuthorize((int)EUserRole.STUDENT)]
        public async Task<IActionResult> DeleteNode(long nodeId)
        {
            var result = await _roadmapService.DeleteNodeAsync(nodeId);
            if (!result) return NotFound();
            return NoContent();
        }


        /// <summary>
        /// View roadmap with basic data in roadmap and all node related and applied data structure
        /// </summary>
        [HttpGet("{roadmapId}/graph")]
        public async Task<IActionResult> GetRoadmapGraph(long roadmapId)
        {
            var graph = await _roadmapService.GetRoadmapAsGraphDtoAsync(roadmapId);
            if (graph == null) return NotFound();
            return Ok(graph);
        }

        /// <summary>
        /// Create a link between two nodes
        /// </summary>
        [HttpPost("link")]
        [AuditLog(Tag = "CREATE_NODE_LINK")]
        [PermissionAuthorize((int)EUserRole.STUDENT)]
        public async Task<IActionResult> CreateLink([FromQuery] long fromNodeId, [FromQuery] long toNodeId)
        {
            var link = await _roadmapService.CreateLinkAsync(fromNodeId, toNodeId);
            if (link == null) return BadRequest("Invalid node ids");
            return Ok(link);
        }

        /// <summary>
        /// Delete a link between nodes
        /// </summary>
        [HttpDelete("link/{linkId}")]
        [AuditLog(Tag = "DELETE_NODE_LINK")]
        [PermissionAuthorize((int)EUserRole.STUDENT)]
        public async Task<IActionResult> DeleteLink(long linkId)
        {
            var result = await _roadmapService.DeleteLinkAsync(linkId);
            if (!result) return NotFound();
            return NoContent();
        }



        /// <summary>
        /// View node by id
        /// </summary>
        [HttpGet("node/{nodeId}")]
        [AuditLog(Tag = "VIEW_NODE_DETAIL")]
        public async Task<IActionResult> GetNode(long nodeId)
        {
            var node = await _roadmapService.GetNodeAsync(nodeId);
            if (node == null) return NotFound();
            return Ok(node);
        }

        /// <summary>
        /// Update node by id (links unaffected)
        /// </summary>
        [HttpPut("node/{nodeId}")]
        [AuditLog(Tag = "UPDATE_NODE_DATA")]
        [PermissionAuthorize((int)EUserRole.STUDENT)]
        public async Task<IActionResult> UpdateNode(long nodeId, [FromBody] CreateNodeDto dto)
        {
            var node = await _roadmapService.UpdateNodeAsync(nodeId, dto);
            if (node == null) return NotFound();
            return Ok(node);
        }


        [HttpPost("{roadmapId}/nodes/bulk")]
        [AuditLog(Tag = "BULK_REPLACE_NODES")]
        [PermissionAuthorize((int)EUserRole.STUDENT)]
        public async Task<IActionResult> BulkReplaceNodes(long roadmapId, [FromBody] List<CreateNodeDto> nodes)
        {
            if (nodes == null || !nodes.Any())
                return BadRequest("Nodes list cannot be empty.");

            var result = await _roadmapService.ReplaceAllNodesAsync(roadmapId, nodes);
            if (!result) return NotFound();

            return Ok(new { message = "Nodes replaced successfully." });
        }

        [HttpPost("links/bulk")]
        [AuditLog(Tag = "BULK_INSERT_NODE_LINKS")]
        [PermissionAuthorize((int)EUserRole.STUDENT)]
        public async Task<IActionResult> BulkInsertLinks([FromBody] List<RoadmapLinkDto> links)
        {
            if (links == null || !links.Any())
                return BadRequest("Links list cannot be empty.");

            var linkTuples = links.Select(l => (FromNodeId: l.FromNodeId, ToNodeId: l.ToNodeId)).ToList();
            var result = await _roadmapService.BulkInsertLinksAsync(linkTuples);

            return Ok(new { message = "Links inserted successfully.", insertedCount = linkTuples.Count });
        }



        /// <summary>
        /// Use AI to gen nodes
        /// </summary>
        [HttpGet("ai-gen-node")]
        [AuditLog(Tag = "AI_GENERATE_STUDY_ROADMAP_NODE")]
        [PermissionAuthorize((int)EUserRole.STUDENT)]
        public async Task<IActionResult> GenNode([FromQuery] string studentMessage)
        {
            var res = await _roadmapService.GenNodeAsync(AccessToken, studentMessage);
            return Ok(res);
        }



        /// <summary>
        /// Get roadmap id by student profile id -1 if no existed
        /// </summary>
        [HttpGet("get-roadmap-id/{studentProfileId}")]
        public async Task<IActionResult> GetRoadmapIDByStudentProfileID(long studentProfileId)
        {
            var res = await _roadmapService.GetRoadmapIdAsync(studentProfileId);
            return Ok(res);
        }



    }


}