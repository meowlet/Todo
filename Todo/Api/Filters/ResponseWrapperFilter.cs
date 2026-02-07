using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Todo.Api.Models;

namespace Todo.Api.Filters;

public class ResponseWrapperFilter : IAsyncResultFilter
{
    private const string DefaultSuccessMessage = "Operation completed successfully";

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is ObjectResult objectResult && IsSuccessStatusCode(objectResult.StatusCode))
        {
            var message = GetSuccessMessage(context);
            objectResult.Value = new ApiResponse<object>(objectResult.Value, message);
        }

        await next();
    }

    private static bool IsSuccessStatusCode(int? statusCode)
    {
        var code = statusCode ?? 200;
        return code is >= 200 and < 300;
    }

    private static string GetSuccessMessage(ResultExecutingContext context)
    {
        var attribute = context.ActionDescriptor.EndpointMetadata
            .OfType<SuccessMessageAttribute>()
            .FirstOrDefault();

        return attribute?.Message ?? DefaultSuccessMessage;
    }
}
