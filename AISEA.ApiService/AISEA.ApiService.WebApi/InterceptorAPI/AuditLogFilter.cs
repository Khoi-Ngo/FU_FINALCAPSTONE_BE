using System.Reflection;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.Interfaces;
using AISEA.ApiService.SHARED.PropConfigs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;


namespace AISEA.ApiService.WebApi.InterceptorAPI;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class AuditLogAttribute : Attribute
{
    public string Tag { get; set; }
    public string Description { get; set; }
}

// The action filter that intercepts requests and queues audit logs in the background
public class AuditLogFilter : IAsyncActionFilter
{
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly IJWTService _jWTService;
    private readonly EndpointSettings _endpointSettings;

    public AuditLogFilter(IBackgroundTaskQueue taskQueue, IJWTService jWTService, EndpointSettings endpointSettings)
    {
        _taskQueue = taskQueue;
        _jWTService = jWTService;
        _endpointSettings = endpointSettings;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Execute the action method
        var resultContext = await next();

        #region client request information
        // Get request-level info
        var ipAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = context.HttpContext.Request.Headers["User-Agent"].FirstOrDefault() ?? "Unknown";


        var accessToken = context.HttpContext.Request.Headers[_endpointSettings.AccessTokenPropName].FirstOrDefault()?.Replace("Bearer ", "");

        // Default user info placeholders
        string userName = "Anonymous";
        string firstName = null;
        string lastName = null;
        long? roleId = null;
        string email = null;
        long? userId = null;



        try
        {
            if (!string.IsNullOrEmpty(accessToken))
            {
                // Use your JWTService to extract claims safely
                userName = _jWTService.GetUsernameFromToken(accessToken);
                firstName = _jWTService.GetFirstNameFromToken(accessToken);
                lastName = _jWTService.GetLastNameFromToken(accessToken);
                roleId = _jWTService.GetRoleIdFromToken(accessToken);
                email = _jWTService.GetEmailFromToken(accessToken);
                userId = _jWTService.GetUserIdFromToken(accessToken);
            }
        }
        catch
        {
            // Token invalid or expired - ignore or log if needed
        }



        #endregion



        // Check if the action method has the AuditLogAttribute
        if (context.ActionDescriptor is ControllerActionDescriptor actionDescriptor)
        {
            var attribute = actionDescriptor.MethodInfo.GetCustomAttribute<AuditLogAttribute>();
            if (attribute != null)
            {
                var tag = attribute.Tag;
                var description = attribute.Description;
                bool isSuccessAction = IsSuccessResponse(resultContext);

                // Queue the audit log creation in the background to avoid blocking the HTTP response
                _taskQueue.QueueBackgroundWorkItem(async (sp, token) =>
                {
                    var auditLogRepo = sp.GetRequiredService<AuditLogRepository>();
                    await auditLogRepo.CreateAsync(new DAL.Entities.AuditLog
                    {
                        Tag = tag,
                        Description = description,
                        IsSuccessAction = isSuccessAction,
                        IPAddress = ipAddress,
                        UserAgent = userAgent,
                        UserName = userName,
                        FirstName = firstName,
                        LastName = lastName,
                        RoleId = roleId,
                        Email = email,
                        UserId = userId
                    });
                });
            }
        }
    }

    private bool IsSuccessResponse(ActionExecutedContext context)
    {
        // Check if the response is successful (200-299) or failed (client/server errors)
        if (context.Exception != null)
        {
            return false; // Server error due to unhandled exception
        }

        if (context.Result is IActionResult result)
        {
            if (result is StatusCodeResult statusCodeResult)
            {
                return statusCodeResult.StatusCode >= 200 && statusCodeResult.StatusCode < 300;
            }
            if (result is ObjectResult objectResult)
            {
                return objectResult.StatusCode == null || (objectResult.StatusCode >= 200 && objectResult.StatusCode < 300);
            }
            // Default to true for results like OkResult, which imply 200 OK
            return true;
        }
        return false;
    }
}