namespace Todo.Application.Exceptions;

public class AppException(ErrorCode errorCode, string? customMessage = null, object? details = null)
    : Exception(customMessage ?? errorCode.DefaultMessage)
{
    public ErrorCode ErrorCode { get; } = errorCode;
    public string CustomMessage { get; } = customMessage ?? errorCode.DefaultMessage;
    public object? Details { get; } = details;
}
