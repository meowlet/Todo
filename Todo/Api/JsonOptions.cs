using System.Text.Json;

namespace Todo.Api;

public static class JsonOptions
{
    public static readonly JsonSerializerOptions SnakeCase = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };
}
