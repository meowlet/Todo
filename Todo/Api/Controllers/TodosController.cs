using Microsoft.AspNetCore.Mvc;
using Todo.Api.Filters;
using Todo.Api.Models;
using Todo.Application.DTOs;
using Todo.Application.Services;

namespace Todo.Api.Controllers;

[ApiController]
[Route("api/todos")]
public class TodosController : ControllerBase
{
    private readonly ITodoService _todoService;

    public TodosController(ITodoService todoService)
    {
        _todoService = todoService;
    }

    [HttpGet]
    [SuccessMessage("Todos retrieved successfully")]
    public async Task<IActionResult> GetAll([FromQuery] int? page, [FromQuery(Name = "page_size")] int? pageSize)
    {
        if (page.HasValue && pageSize.HasValue)
        {
            var (items, totalCount) = await _todoService.GetPaginatedAsync(page.Value, pageSize.Value);
            var paginatedResult = new PaginatedResponse<TodoResponseDto>
            {
                Items = items,
                Pagination = PaginationMeta.Create(page.Value, pageSize.Value, totalCount)
            };
            return Ok(paginatedResult);
        }
        var todos = await _todoService.GetAllAsync();
        return Ok(todos);
    }

    [HttpGet("{id:guid}")]
    [SuccessMessage("Todo retrieved successfully")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var todo = await _todoService.GetByIdAsync(id);
        return Ok(todo);
    }

    [HttpPost]
    [SuccessMessage("Todo created successfully")]
    public async Task<IActionResult> Create([FromBody] TodoCreateDto dto)
    {
        var todo = await _todoService.CreateAsync(dto);
        return StatusCode(201, todo);
    }

    [HttpPut("{id:guid}")]
    [SuccessMessage("Todo updated successfully")]
    public async Task<IActionResult> Update(Guid id, [FromBody] TodoUpdateDto dto)
    {
        var todo = await _todoService.UpdateAsync(id, dto);
        return Ok(todo);
    }

    [HttpDelete("{id:guid}")]
    [SuccessMessage("Todo deleted successfully")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _todoService.DeleteAsync(id);
        return Ok();
    }
}
