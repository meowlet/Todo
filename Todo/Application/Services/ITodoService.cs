using Todo.Application.DTOs;

namespace Todo.Application.Services;

public interface ITodoService
{
    Task<IEnumerable<TodoResponseDto>> GetAllAsync();
    Task<(IEnumerable<TodoResponseDto> Items, int TotalCount)> GetPaginatedAsync(int page, int pageSize);
    Task<TodoResponseDto> GetByIdAsync(Guid id);
    Task<TodoResponseDto> CreateAsync(TodoCreateDto dto);
    Task<TodoResponseDto> UpdateAsync(Guid id, TodoUpdateDto dto);
    Task DeleteAsync(Guid id);
}
