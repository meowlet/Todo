namespace Todo.Api.Filters;

[AttributeUsage(AttributeTargets.Method)]
public class SuccessMessageAttribute : Attribute
{
    public string Message { get; }

    public SuccessMessageAttribute(string message)
    {
        Message = message;
    }
}
