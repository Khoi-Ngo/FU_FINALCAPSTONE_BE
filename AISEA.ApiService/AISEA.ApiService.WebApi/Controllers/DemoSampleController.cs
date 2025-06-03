using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISEA.ApiService.BAL.Services;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.WebApi.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DemoSampleController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DemoSampleService _demoSampleService;

        public DemoSampleController(IConfiguration configuration, DemoSampleService demoSampleService)
        {
            _configuration = configuration;
            _demoSampleService = demoSampleService;
        }

        [HttpGet]
        public IActionResult Demo()
        {
            var dummyValue = _configuration["DummyKey"];
            return Ok(new { DummyKey = dummyValue });
        }
        [HttpGet("with-role-specified")]
        [PermissionAuthorize((int)EUserRole.ADMIN, (int)EUserRole.MANAGER)]
        public IActionResult DemoWithRoleSpecified()
        {
            var dummyValue = _configuration["DummyKey"];
            return Ok(new { DummyKey = dummyValue + "With Role Specified Version" });
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
            var res = await _demoSampleService.DemoRefreshTokenWithRedis();
            return Ok(res);
        }

        [HttpGet("logout")]
        public async Task<IActionResult> DemoLogoutWithRedis()
        {
            await _demoSampleService.DemoLogoutWithRedis();
            return Ok();
        }

    }
    public class DemoLoginRequest
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }

}