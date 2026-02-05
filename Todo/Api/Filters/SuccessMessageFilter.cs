using Microsoft.AspNetCore.Mvc.Filters;

namespace Todo.Api.Filters;

public class SuccessMessageFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        var attribute = context.ActionDescriptor.EndpointMetadata
            .OfType<SuccessMessageAttribute>()
            .FirstOrDefault();

        if (attribute != null)
        {
            context.HttpContext.Items["SuccessMessage"] = attribute.Message;
        }
    }
}
