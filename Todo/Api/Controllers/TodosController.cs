using Microsoft.AspNetCore.Mvc;
using Todo.Api.Filters;
using Todo.Api.Models;
using Todo.Application.DTOs;
using Todo.Application.Services;

namespace Todo.Api.Controllers;

[ApiController]
[Route("api/todos")]
public class TodosController(ITodoService todoService) : ControllerBase
{
    [HttpGet]
    [SuccessMessage("Todos retrieved successfully")]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery(Name = "page_size")] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var (items, actualPage, actualPageSize, totalCount) = await todoService.GetAllAsync(page, pageSize, cancellationToken);

        var result = new PaginatedResponse<TodoResponseDto>
        {
            Items = items,
            Pagination = PaginationMeta.Create(actualPage, actualPageSize, totalCount)
        };

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [SuccessMessage("Todo retrieved successfully")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var todo = await todoService.GetByIdAsync(id, cancellationToken);
        return Ok(todo);
    }

    [HttpPost]
    [SuccessMessage("Todo created successfully")]
    public async Task<IActionResult> Create([FromBody] TodoCreateDto dto, CancellationToken cancellationToken)
    {
        var todo = await todoService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = todo.Id }, todo);
    }

    [HttpPatch("{id:guid}")]
    [SuccessMessage("Todo updated successfully")]
    public async Task<IActionResult> Update(Guid id, [FromBody] TodoUpdateDto dto, CancellationToken cancellationToken)
    {
        var todo = await todoService.UpdateAsync(id, dto, cancellationToken);
        return Ok(todo);
    }

    [HttpDelete("{id:guid}")]
    [SuccessMessage("Todo deleted successfully")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await todoService.DeleteAsync(id, cancellationToken);
        return Ok();
    }
}
