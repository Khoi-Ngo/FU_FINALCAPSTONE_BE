using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Text.Json;
using System.Threading.Tasks;
using AISEA.ApiService.SHARED.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AISEA.ApiService.SHARED.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // Continue pipeline
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, _logger);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger logger)
    {
        context.Response.ContentType = "application/json";

        int statusCode;
        string message;

        switch (exception)
        {
            case InvalidCredentialException:
                statusCode = (int)HttpStatusCode.BadRequest;
                message = exception.Message;
                break;

            case UnauthorizedAccessException:
                statusCode = (int)HttpStatusCode.Unauthorized;
                message = "Unauthorized access.";
                break;

            case ValidationException validation:
                statusCode = (int)HttpStatusCode.BadRequest;
                message = validation.Message;
                break;

            case InvalidRefreshToken:
                statusCode = (int)HttpStatusCode.BadRequest;
                message = exception.Message;
                break;
            case InvalidCGoogleTokenException:
            case EmptyTokenGoogleLoginException:
                statusCode = (int)HttpStatusCode.Unauthorized;
                message = exception.Message;
                break;
            case InvalidUserCreatedException:
                statusCode = (int)HttpStatusCode.BadRequest;
                message = exception.Message;
                break;

            default:
                logger.LogError(exception, "Unhandled exception");
                statusCode = (int)HttpStatusCode.InternalServerError;
                message = "An unexpected error occurred.";
                break;
        }

        context.Response.StatusCode = statusCode;

        var result = JsonSerializer.Serialize(new
        {
            status = statusCode,
            message
        });

        await context.Response.WriteAsync(result);
    }
}