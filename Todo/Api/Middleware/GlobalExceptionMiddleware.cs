using System.Text.Json;
using Todo.Api.Models;
using Todo.Application.Exceptions;

namespace Todo.Api.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
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
        var json = JsonSerializer.Serialize(response, JsonOptions.SnakeCase);
        
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
        var json = JsonSerializer.Serialize(response, JsonOptions.SnakeCase);
        
        await context.Response.WriteAsync(json);
    }

    private static int MapErrorCodeToHttpStatus(ErrorCode errorCode)
    {
        return errorCode switch
        {
            var e when e == ErrorCode.ValidationError => 400,
            var e when e == ErrorCode.ResourceNotFound => 404,
            var e when e == ErrorCode.ResourceAlreadyExists => 409,
            var e when e == ErrorCode.InternalError => 500,
            _ => 500
        };
    }
}
