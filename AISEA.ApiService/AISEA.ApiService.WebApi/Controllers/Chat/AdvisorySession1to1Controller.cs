using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISEA.ApiService.BAL.Services.Chat;
using AISEA.ApiService.SHARED.DTOs.Requests.Chat;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.Chat
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdvisorySession1to1Controller : BaseController
    {
        private readonly AdvisorySession1to1Service _advisorySession1To1Service;
        private readonly ChatService _chatService;

        public AdvisorySession1to1Controller(EndpointSettings endpointSettings, AdvisorySession1to1Service advisorySession1To1Service, ChatService chatService) : base(endpointSettings)
        {
            _advisorySession1To1Service = advisorySession1To1Service;
            _chatService = chatService;
        }

        /// <summary>
        /// Delete Chat Session
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            await _advisorySession1To1Service.DeleteAsync(id, AccessToken);
            return Ok("Delete successfully");
        }

        /// <summary>
        /// Initialize the chat session with Staffs User
        /// </summary>
        [HttpPost("human")]
        public async Task<IActionResult> InitHumanChatSessionAsync([FromBody] InitHumanChatSessioRequest request)
        {
            var res = await _chatService.InitHumanChatSessionAsync(request, AccessToken);
            return Ok(res);
        }

    }
}



#region Ignore


// /// <summary>
// /// Get All ChatSessions
// /// </summary>

// [HttpGet]
// public async Task<IActionResult> GetAllByStudentSelfAsync([FromQuery] PaginationRequest request)
// {
//     var res = await _advisorySession1To1Service.GetAllByStudentSelfAsync(request, AccessToken);
//     return Ok(res);
// }
#endregion