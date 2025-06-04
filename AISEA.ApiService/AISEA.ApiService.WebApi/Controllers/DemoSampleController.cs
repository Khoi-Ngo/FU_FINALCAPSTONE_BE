using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISEA.ApiService.BAL.Services;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.Filters;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DemoSampleController : BaseController
    {
        private readonly DemoSampleService _demoSampleService;

        private readonly EndpointSettings _endpointSettings;

        public DemoSampleController(
            DemoSampleService demoSampleService,
            EndpointSettings endpointSettings
        ) : base(endpointSettings)
        {
            _demoSampleService = demoSampleService;
            _endpointSettings = endpointSettings;
        }

        [HttpGet]
        public IActionResult Demo()
        {
            return Ok(new { NonKey = "asdksajdsakjdaskjdsa" });
        }
        [HttpGet("with-role-specified")]
        [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.MANAGER)]
        public IActionResult DemoWithRoleSpecified()
        {
            return Ok(new { NonKey = "asdksajdsakjdaskjdsa" + "With Role Specified Version" });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> DemoLoginWithRedis([FromBody] DemoLoginRequest request)
        {
            var res = await _demoSampleService.DemoLoginWithRedis(request.UserName, request.Password);
            return Ok(res);
        }


        [HttpGet("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> DemoRefreshTokenWithRedis()
        {
            var res = await _demoSampleService.DemoRefreshTokenWithRedis(AccessToken, RefreshToken);
            return Ok(res);
        }

        [HttpGet("logout")]
        public async Task<IActionResult> DemoLogoutWithRedis()
        {
            await _demoSampleService.DemoLogoutWithRedis(AccessToken);
            return Ok();
        }

    }
    public class DemoLoginRequest
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }

}