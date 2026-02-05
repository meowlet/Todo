using System.Text.Json;
using Todo.Api.Models;

namespace Todo.Api.Middleware;

public class ResponseTransformMiddleware
{
    private readonly RequestDelegate _next;
    private readonly JsonSerializerOptions _jsonOptions;

    public ResponseTransformMiddleware(RequestDelegate next)
    {
        _next = next;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var originalBodyStream = context.Response.Body;

        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);
        }
        catch
        {
            context.Response.Body = originalBodyStream;
            throw;
        }

        var statusCode = context.Response.StatusCode;
        
        if (statusCode >= 200 && statusCode < 300)
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            var successMessage = context.Items["SuccessMessage"] as string ?? "Operation completed successfully";
            
            object? data = null;
            if (!string.IsNullOrEmpty(body))
            {
                try
                {
                    data = JsonSerializer.Deserialize<JsonElement>(body);
                }
                catch
                {
                    data = body;
                }
            }

            var wrappedResponse = new ApiResponse<object>(data, successMessage);
            var wrappedJson = JsonSerializer.Serialize(wrappedResponse, _jsonOptions);

            context.Response.Body = originalBodyStream;
            context.Response.ContentType = "application/json";
            context.Response.ContentLength = null;
            
            await context.Response.WriteAsync(wrappedJson);
        }
        else
        {
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
            context.Response.Body = originalBodyStream;
        }
    }
}
