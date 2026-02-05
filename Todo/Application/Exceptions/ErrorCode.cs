namespace Todo.Application.Exceptions;

public class ErrorCode
{
    public string Code { get; }
    public string DefaultMessage { get; }

    private ErrorCode(string code, string defaultMessage)
    {
        Code = code;
        DefaultMessage = defaultMessage;
    }

    public static readonly ErrorCode ValidationError = new("VALIDATION_ERROR", "Validation failed");
    public static readonly ErrorCode ResourceNotFound = new("RESOURCE_NOT_FOUND", "Resource not found");
    public static readonly ErrorCode ResourceAlreadyExists = new("RESOURCE_ALREADY_EXISTS", "Resource already exists");
    public static readonly ErrorCode InternalError = new("INTERNAL_ERROR", "An internal error occurred");
}
