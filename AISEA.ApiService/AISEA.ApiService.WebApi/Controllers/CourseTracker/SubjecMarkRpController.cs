using AISEA.ApiService.BAL.Services.CourseTracker;
using AISEA.ApiService.SHARED.Const.Enums;
using AISEA.ApiService.SHARED.DTOs.Requests.MarkReport;
using AISEA.ApiService.SHARED.Filters;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.SHARED.PropConfigs;
using AISEA.ApiService.WebApi.Base;
using AISEA.ApiService.WebApi.InterceptorAPI;
using Microsoft.AspNetCore.Mvc;

namespace AISEA.ApiService.WebApi.Controllers.CourseTracker
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubjecMarkRpController : BaseController
    {
        private readonly MarkReportService _markReportService;
        private readonly IBackgroundTaskQueue _taskQueue;

        public SubjecMarkRpController(EndpointSettings endpointSettings
        , MarkReportService markReportService
        , IBackgroundTaskQueue taskQueue) : base(endpointSettings)
        {
            _markReportService = markReportService;
            _taskQueue = taskQueue;
        }


        ///<summary>
        /// Import multiple mark reports for a joined subject
        /// </summary>
        [HttpPost("{joinedSubjectID}")]
        [PermissionAuthorize((int)EUserRole.ACADEMIC_STAFF)]
        [AuditLog(Tag = "IMPORT_MARK_REPORT")]
        public async Task<IActionResult> Import([FromBody] List<CommandMarkRpRequest> request, long joinedSubjectID)
        {
            await _markReportService.ImportAsync(request, joinedSubjectID, AccessToken);

            _taskQueue.QueueBackgroundWorkItem(async (sp, token) =>
{
    var qMarkReportService = sp.GetRequiredService<MarkReportService>();
    await qMarkReportService.UpdateStatusPassedAsync(joinedSubjectID);
});

            return Ok("Import successfully");
        }



        ///<summary>
        /// Delete single mark report
        /// </summary>
        [HttpDelete("{id}")]
        [PermissionAuthorize((int)EUserRole.ACADEMIC_STAFF)]
        [AuditLog(Tag = "DELETE_MARK_REPORT")]
        public async Task<IActionResult> Delete(long id)
        {

            var needCheckJoinedSubjectID = await _markReportService.DeleteAsync(id);
            _taskQueue.QueueBackgroundWorkItem(async (sp, token) =>
        {
            var qMarkReportService = sp.GetRequiredService<MarkReportService>();
            await qMarkReportService.UpdateStatusPassedAsync(needCheckJoinedSubjectID);
        });
            return Ok("Delete successfully");
        }

        ///<summary>
        /// UPDATE single mark report
        /// </summary>
        [HttpPut("{id}")]
        [PermissionAuthorize((int)EUserRole.ACADEMIC_STAFF)]
        [AuditLog(Tag = "UPDATE_MARK_REPORT")]
        public async Task<IActionResult> Update(long id, [FromBody] CommandMarkRpRequest request)
        {

            var needCheckJoinedSubjectID = await _markReportService.UpdateAsync(id, request);
            _taskQueue.QueueBackgroundWorkItem((Func<IServiceProvider, CancellationToken, Task>)(async (sp, token) =>
        {
            var qMarkReportService = sp.GetRequiredService<MarkReportService>();
            await qMarkReportService.UpdateStatusPassedAsync(needCheckJoinedSubjectID);
        }));
            return Ok("Update successfully");
        }



        ///<summary>
        /// Get all mark reports associated with a joined subject
        ///  with user accessing verification (NOT APPLIED -> TradeOff Performance)
        /// </summary>
        [HttpGet("{joinedSubjectId}")]
        [AuditLog(Tag = "VIEW_MARK_REPORT")]
        public async Task<IActionResult> View(long joinedSubjectId)
        {
            var res = await _markReportService.ViewByJoinedSubjectAsync(joinedSubjectId);
            return Ok(res);
        }

        ///<summary>
        /// Get transcript
        /// </summary>
        [HttpGet("personal-academic-transcript")]
        [PermissionAuthorize((int)EUserRole.STUDENT)]
        [AuditLog(Tag = "VIEW_TRANSCRIPT")]
        public async Task<IActionResult> ViewPersonalTranscript()
        {
            var res = await _markReportService.ViewPersonalTranscriptAsync(AccessToken);
            return Ok(res);
        }



        ///<summary>
        /// Get template for importing the mark reports
        /// </summary>
        [HttpGet("view-template-import")]
        [PermissionAuthorize((int)EUserRole.ACADEMIC_STAFF)]
        [AuditLog(Tag = "VIEW_TEMPLATE_IMPORT_MARK_REPORT")]
        public async Task<IActionResult> ViewTemplateImport([FromQuery] string subjectCode, [FromQuery] string subjectVersionCode)
        {
            var res = await _markReportService.ViewTemplateImportMarkAsync(subjectCode, subjectVersionCode);
            return Ok(res);
        }



    }
}