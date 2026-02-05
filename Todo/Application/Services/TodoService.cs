using Microsoft.EntityFrameworkCore;
using Todo.Application.DTOs;
using Todo.Application.Exceptions;
using Todo.Domain.Entities;
using Todo.Infrastructure.Data;

namespace Todo.Application.Services;

public class TodoService : ITodoService
{
    private readonly AppDbContext _context;

    public TodoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TodoResponseDto>> GetAllAsync()
    {
        var todos = await _context.Todos
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
        return todos.Select(MapToDto);
    }

    public async Task<(IEnumerable<TodoResponseDto> Items, int TotalCount)> GetPaginatedAsync(int page, int pageSize)
    {
        var query = _context.Todos.OrderByDescending(t => t.CreatedAt);
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (items.Select(MapToDto), totalCount);
    }

    public async Task<TodoResponseDto> GetByIdAsync(Guid id)
    {
        var todo = await _context.Todos.FindAsync(id);
        if (todo == null)
        {
            throw AppException.NotFound("Todo", id);
        }
        return MapToDto(todo);
    }

    public async Task<TodoResponseDto> CreateAsync(TodoCreateDto dto)
    {
        var todo = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Todos.Add(todo);
        await _context.SaveChangesAsync();
        return MapToDto(todo);
    }

    public async Task<TodoResponseDto> UpdateAsync(Guid id, TodoUpdateDto dto)
    {
        var todo = await _context.Todos.FindAsync(id);
        if (todo == null)
        {
            throw AppException.NotFound("Todo", id);
        }
        todo.Title = dto.Title;
        todo.Description = dto.Description;
        todo.IsCompleted = dto.IsCompleted;
        todo.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return MapToDto(todo);
    }

    public async Task DeleteAsync(Guid id)
    {
        var todo = await _context.Todos.FindAsync(id);
        if (todo == null)
        {
            throw AppException.NotFound("Todo", id);
        }
        _context.Todos.Remove(todo);
        await _context.SaveChangesAsync();
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
