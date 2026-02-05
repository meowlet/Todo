using Todo.Application.DTOs;

namespace Todo.Application.Services;

public interface ITodoService
{
    Task<(IEnumerable<TodoResponseDto> Items, int Page, int PageSize, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<TodoResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TodoResponseDto> CreateAsync(TodoCreateDto dto, CancellationToken cancellationToken = default);
    Task<TodoResponseDto> UpdateAsync(Guid id, TodoUpdateDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
