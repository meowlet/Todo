using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Todo.Api.Filters;
using Todo.Api.Middleware;
using Todo.Application.Exceptions;
using Todo.Application.Services;
using Todo.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Configure JSON serialization with snake_case
builder.Services.AddControllers(options =>
{
    options.Filters.Add<SuccessMessageFilter>();
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
});

// Configure validation error response
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .ToDictionary(
                e => JsonNamingPolicy.SnakeCaseLower.ConvertName(e.Key),
                e => e.Value?.Errors.First().ErrorMessage ?? "Invalid value"
            );

        throw AppException.ValidationFailed(errors);
    };
});

// Configure Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Services
builder.Services.AddScoped<ITodoService, TodoService>();

// Configure OpenAPI/Swagger
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure middleware pipeline
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<ResponseTransformMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
