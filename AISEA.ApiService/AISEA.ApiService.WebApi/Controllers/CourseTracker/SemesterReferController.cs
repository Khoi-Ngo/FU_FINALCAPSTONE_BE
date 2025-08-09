using AISEA.ApiService.BAL.Services.CourseTracker;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.Pagin;
using AISEA.ApiService.SHARED.Filters;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.CourseTracker;

[ApiController]
[Route("api/[controller]")]
public class SemesterReferController : BaseController
{
    private readonly SemesterReferService _semesterReferService;
    public SemesterReferController(EndpointSettings endpointSettings, SemesterReferService semesterReferService) : base(endpointSettings)
    {
        _semesterReferService = semesterReferService;
    }

    ///<summary>
    /// Get All Semesters
    /// </summary>
    [HttpGet]
    [PermissionAuthorize((int)EUserRole.ACADEMIC_STAFF, (int)EUserRole.ADMIN, (int)EUserRole.MANAGER)]
    public async Task<IActionResult> GetAllAsyncPaged([FromQuery] PaginationRequest request)
    {
        var result = await _semesterReferService.GetAllAsyncPaged(request);
        return Ok(result);
    }

    ///<summary>
    /// Get All Block Types
    /// </summary>
    [HttpGet("block-types")]
    [PermissionAuthorize((int)EUserRole.ACADEMIC_STAFF, (int)EUserRole.ADMIN, (int)EUserRole.MANAGER)]
    public async Task<IActionResult> GetAllBlockTypesAsync([FromQuery] PaginationRequest request)
    {
        var allBlockTypes = Enum.GetValues(typeof(ESemesterStudyBlockType))
            .Cast<ESemesterStudyBlockType>()
            .Select(bt => new
            {
                Id = (int)bt,
                Name = bt.ToString()
            })
            .ToList();

        var pagedData = allBlockTypes
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var result = new
        {
            Data = pagedData,
            TotalCount = allBlockTypes.Count
        };

        return Ok(result);
    }

}