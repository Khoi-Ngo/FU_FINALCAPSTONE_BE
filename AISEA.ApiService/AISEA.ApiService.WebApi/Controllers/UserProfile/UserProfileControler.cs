using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISEA.ApiService.BAL.Services.SystemProfile;
using AISEA.ApiService.DAL.Entities;
using AISEA.ApiService.SHARED.DTOs.Requests.SystemProfile;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.UserProfile
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserProfileControler : BaseController
    {
        private readonly StudentProfileService _studentProfileService;
        private readonly StaffProfileService _staffProfileService;
        public UserProfileControler(EndpointSettings endpointSettings, StudentProfileService studentProfileService, StaffProfileService staffProfileService) : base(endpointSettings)
        {
            _staffProfileService = staffProfileService;
            _studentProfileService = studentProfileService;
        }

        #region Student Profile

        /// <summary>
        /// Create student profile with existed user in the system
        /// </summary>
        [HttpPost("student")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateStudentProfileRequest request)
        {
            await _studentProfileService.CreateAsync(request);
            return Ok("Created Successfully");
        }



        #endregion


        #region Staff Profile

        /// <summary>
        /// Create staff profile with existed user in the system
        /// </summary>
        [HttpPost("staff")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateStaffProfileRequest request)
        {
            await _staffProfileService.CreateAsync(request);
            return Ok("Created Successfully");
        }
        #endregion
    }
}