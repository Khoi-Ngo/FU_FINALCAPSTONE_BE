using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DemoSampleController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public DemoSampleController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Demo()
        {
            var dummyValue = _configuration["DummyKey"];
            return Ok(new { DummyKey = dummyValue });
        }
    }
}