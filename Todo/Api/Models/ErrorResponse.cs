namespace Todo.Api.Models;

public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public ErrorInfo Error { get; set; } = new();

    public ErrorResponse() { }

    public ErrorResponse(string message, string code, object? details = null)
    {
        Message = message;
        Error = new ErrorInfo
        {
            Code = code,
            Details = details
        };
    }
}

public class ErrorInfo
{
    public string Code { get; set; } = string.Empty;
    public object? Details { get; set; }
}
