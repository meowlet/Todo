namespace Todo.Application.Exceptions;

public class AppException : Exception
{
    public ErrorCode ErrorCode { get; }
    public string CustomMessage { get; }
    public object? Details { get; }

    public AppException(ErrorCode errorCode, string? customMessage = null, object? details = null)
        : base(customMessage ?? errorCode.DefaultMessage)
    {
        ErrorCode = errorCode;
        CustomMessage = customMessage ?? errorCode.DefaultMessage;
        Details = details;
    }

    public static AppException NotFound(string resourceName, object? id = null)
    {
        var message = id != null 
            ? $"{resourceName} with id '{id}' not found" 
            : $"{resourceName} not found";
        return new AppException(ErrorCode.ResourceNotFound, message);
    }

    public static AppException ValidationFailed(object details)
    {
        return new AppException(ErrorCode.ValidationError, "Validation failed", details);
    }
}
