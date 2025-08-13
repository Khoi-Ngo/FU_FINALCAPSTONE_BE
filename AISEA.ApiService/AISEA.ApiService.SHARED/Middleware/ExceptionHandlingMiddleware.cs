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
        string result;

        switch (exception)
        {
            case InvalidCredentialException:
                statusCode = (int)HttpStatusCode.BadRequest;
                result = JsonSerializer.Serialize(new
                {
                    status = statusCode,
                    message = exception.Message
                });
                break;

            case UnauthorizedAccessException:
                statusCode = (int)HttpStatusCode.Unauthorized;
                result = JsonSerializer.Serialize(new
                {
                    status = statusCode,
                    message = "Unauthorized access."
                });
                break;

            case ValidationException validation:
                statusCode = (int)HttpStatusCode.BadRequest;
                result = JsonSerializer.Serialize(new
                {
                    status = statusCode,
                    message = validation.Message
                });
                break;

            case InvalidRefreshToken:
                statusCode = (int)HttpStatusCode.BadRequest;
                result = JsonSerializer.Serialize(new
                {
                    status = statusCode,
                    message = exception.Message
                });
                break;

            case InvalidCGoogleTokenException:
            case EmptyTokenGoogleLoginException:
                statusCode = (int)HttpStatusCode.Unauthorized;
                result = JsonSerializer.Serialize(new
                {
                    status = statusCode,
                    message = exception.Message
                });
                break;

            case InvalidUserCreatedException:
                statusCode = (int)HttpStatusCode.BadRequest;
                result = JsonSerializer.Serialize(new
                {
                    status = statusCode,
                    message = exception.Message
                });
                break;

            case InvalidDataInput:
                statusCode = (int)HttpStatusCode.BadRequest;
                result = JsonSerializer.Serialize(new
                {
                    status = statusCode,
                    message = exception.Message
                });
                break;

            case InvalidAccessTokenException:
                statusCode = (int)HttpStatusCode.Unauthorized;
                result = JsonSerializer.Serialize(new
                {
                    status = statusCode,
                    message = exception.Message
                });
                break;

            case KeyNotFoundException keyNotFound:
                statusCode = (int)HttpStatusCode.NotFound;
                result = JsonSerializer.Serialize(new
                {
                    status = statusCode,
                    message = keyNotFound.Message
                });
                break;

            case InvalidAccessSession:
                statusCode = (int)HttpStatusCode.NotFound;
                result = JsonSerializer.Serialize(new
                {
                    status = statusCode,
                    message = exception.Message
                });
                break;

            case NotFoundException:
                statusCode = (int)HttpStatusCode.NotFound;
                result = JsonSerializer.Serialize(new
                {
                    status = statusCode,
                    message = exception.Message
                });
                break;

            case InvalidAccessBookingAvailability:
                statusCode = (int)HttpStatusCode.Unauthorized;
                result = JsonSerializer.Serialize(new
                {
                    status = statusCode,
                    message = exception.Message
                });
                break;
            case InvalidAccessMeeting:
                statusCode = (int)HttpStatusCode.Unauthorized;
                result = JsonSerializer.Serialize(new
                {
                    status = statusCode,
                    message = exception.Message
                });
                break;

            case InvalidAccessUserException:
                statusCode = (int)HttpStatusCode.Unauthorized;
                result = JsonSerializer.Serialize(new
                {
                    status = statusCode,
                    message = exception.Message
                });
                break;

            case BookingAvaiDuplicateEx:
            case BookingAvaiOverlapEx:
                statusCode = (int)HttpStatusCode.BadRequest;
                result = JsonSerializer.Serialize(new
                {
                    status = statusCode,
                    message = exception.Message
                });
                break;

            case OnHolidayException holidayEx:
                statusCode = (int)HttpStatusCode.BadRequest;
                result = JsonSerializer.Serialize(new
                {
                    status = statusCode,
                    message = holidayEx.Message,
                    holidays = holidayEx.Holidays
                });
                break;
            case InvalidAccessLeaveSche:
                statusCode = (int)HttpStatusCode.BadRequest;
                result = JsonSerializer.Serialize(new
                {
                    status = statusCode,
                    message = exception.Message
                });
                break;
            case InvalidOperationException:
            case LeaveScheduleOverlapEx:
            case NoMatchingBookingAvailabilityEx:
            case LeaveScheduleConflictWithMeetingsEx:
            case InvalidCurMeetingStatException:
                statusCode = (int)HttpStatusCode.BadRequest;
                result = JsonSerializer.Serialize(new
                {
                    status = statusCode,
                    message = exception.Message
                });
                break;
            case InvalidAccessJoinedSubject:
                statusCode = (int)HttpStatusCode.Unauthorized;
                result = JsonSerializer.Serialize(new
                {
                    status = statusCode,
                    message = exception.Message
                });
                break;

            default:
                logger.LogError(exception, "Unhandled exception");
                statusCode = (int)HttpStatusCode.InternalServerError;
                result = JsonSerializer.Serialize(new
                {
                    status = statusCode,
                    message = exception.Message
                });
                break;
        }

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(result);
    }

}