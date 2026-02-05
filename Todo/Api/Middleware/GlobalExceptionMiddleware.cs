using System.Text.Json;
using Todo.Api.Models;
using Todo.Application.Exceptions;

namespace Todo.Api.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException ex)
        {
            await HandleAppExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            await HandleUnknownExceptionAsync(context, ex);
        }
    }

    private async Task HandleAppExceptionAsync(HttpContext context, AppException exception)
    {
        var statusCode = MapErrorCodeToHttpStatus(exception.ErrorCode);
        
        if (statusCode >= 500)
        {
            _logger.LogError(exception, "Application error occurred: {Message}", exception.CustomMessage);
        }
        else
        {
            _logger.LogWarning("Client error occurred: {Code} - {Message}", exception.ErrorCode.Code, exception.CustomMessage);
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = new ErrorResponse(exception.CustomMessage, exception.ErrorCode.Code, exception.Details);
        var json = JsonSerializer.Serialize(response, _jsonOptions);
        
        await context.Response.WriteAsync(json);
    }

    private async Task HandleUnknownExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Unhandled exception occurred");

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = 500;

        var response = new ErrorResponse(
            "An internal error occurred",
            ErrorCode.InternalError.Code,
            null
        );
        var json = JsonSerializer.Serialize(response, _jsonOptions);
        
        await context.Response.WriteAsync(json);
    }

    private static int MapErrorCodeToHttpStatus(ErrorCode errorCode)
    {
        return errorCode.Code switch
        {
            "VALIDATION_ERROR" => 400,
            "RESOURCE_NOT_FOUND" => 404,
            "RESOURCE_ALREADY_EXISTS" => 409,
            "INTERNAL_ERROR" => 500,
            _ => 500
        };
    }
}
