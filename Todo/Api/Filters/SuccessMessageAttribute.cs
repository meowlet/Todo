namespace Todo.Api.Filters;

[AttributeUsage(AttributeTargets.Method)]
public class SuccessMessageAttribute(string message) : Attribute
{
    public string Message { get; } = message;
}
