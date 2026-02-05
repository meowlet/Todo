using Microsoft.EntityFrameworkCore;
using Todo.Application.DTOs;
using Todo.Application.Exceptions;
using Todo.Domain.Entities;
using Todo.Infrastructure.Data;

namespace Todo.Application.Services;

public class TodoService(AppDbContext context) : ITodoService
{
    private const int MaxPageSize = 100;

    public async Task<(IEnumerable<TodoResponseDto> Items, int Page, int PageSize, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);

        var query = context.Todos.OrderByDescending(t => t.CreatedAt);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items.Select(MapToDto), page, pageSize, totalCount);
    }

    public async Task<TodoResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var todo = await context.Todos.FindAsync([id], cancellationToken);
        return todo == null ? throw new AppException(ErrorCode.ResourceNotFound, $"Todo with id '{id}' not found") : MapToDto(todo);
    }

    public async Task<TodoResponseDto> CreateAsync(TodoCreateDto dto, CancellationToken cancellationToken = default)
    {
        var todo = new TodoItem
        {
            Title = dto.Title,
            Description = dto.Description,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.Todos.Add(todo);
        await context.SaveChangesAsync(cancellationToken);
        return MapToDto(todo);
    }

    public async Task<TodoResponseDto> UpdateAsync(Guid id, TodoUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var todo = await context.Todos.FindAsync([id], cancellationToken);
        if (todo == null)
        {
            throw new AppException(ErrorCode.ResourceNotFound, $"Todo with id '{id}' not found");
        }
        todo.Title = dto.Title;
        todo.Description = dto.Description;
        todo.IsCompleted = dto.IsCompleted;
        todo.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync(cancellationToken);
        return MapToDto(todo);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var todo = await context.Todos.FindAsync([id], cancellationToken);
        if (todo == null)
        {
            throw new AppException(ErrorCode.ResourceNotFound, $"Todo with id '{id}' not found");
        }
        context.Todos.Remove(todo);
        await context.SaveChangesAsync(cancellationToken);
    }

    private static TodoResponseDto MapToDto(TodoItem todo)
    {
        return new TodoResponseDto
        {
            Id = todo.Id,
            Title = todo.Title,
            Description = todo.Description,
            IsCompleted = todo.IsCompleted,
            CreatedAt = todo.CreatedAt,
            UpdatedAt = todo.UpdatedAt
        };
    }
}
