using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.UserProfile
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserProfileControler : BaseController
    {
        public UserProfileControler(EndpointSettings endpointSettings) : base(endpointSettings)
        {
        }

        #region Student Profile
        #endregion


        #region Staff Profile
        #endregion
    }
}