using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISEA.ApiService.BAL.Services.Chat;
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

        public AdvisorySession1to1Controller(EndpointSettings endpointSettings, AdvisorySession1to1Service advisorySession1To1Service) : base(endpointSettings)
        {
            _advisorySession1To1Service = advisorySession1To1Service;
        }

        #region Student

        /// <summary>
        /// Get All ChatSessions By StudentSelf
        /// </summary>

        [HttpGet("student")]
        public async Task<IActionResult> GetAllByStudentSelfAsync([FromQuery] PaginationRequest request)
        {
            var res = await _advisorySession1To1Service.GetAllByStudentSelfAsync(request, AccessToken);
            return Ok(res);
        }

        #endregion

        #region Staff

        /// <summary>
        /// Get All Open (Not Assigned ChatSession)
        /// </summary>
        [HttpGet("open")]
        public async Task<IActionResult> GetAllOpenAsync([FromQuery] PaginationRequest request)
        {
            var res = await _advisorySession1To1Service.GetAllOpenAsync(request);
            return Ok(res);
        }

        /// <summary>
        /// Get All Assigned To StaffSelf
        /// </summary>
        [HttpGet("staff")]
        public async Task<IActionResult> GetAllByStaffSelfAsync([FromQuery] PaginationRequest request)
        {
            var res = await _advisorySession1To1Service.GetAllByStaffSelfPagedAsync(request, AccessToken);
            return Ok(res);
        }


        #endregion


        #region General

        /// <summary>
        /// Get ChatSession By Id
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(long id)
        {
            var res = await _advisorySession1To1Service.GetByIdAsync(id, AccessToken);
            return Ok(res);
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

        #endregion
    }
}