using System.Reflection;
using AISEA.ApiService.DAL.Repositories;
using AISEA.ApiService.SHARED.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace AISEA.ApiService.SHARED.Filters;

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

    public AuditLogFilter(IBackgroundTaskQueue taskQueue)
    {
        _taskQueue = taskQueue;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Execute the action method
        var resultContext = await next();

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
                        IsSuccessAction = isSuccessAction 
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