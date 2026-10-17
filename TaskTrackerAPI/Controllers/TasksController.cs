using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TaskTrackerAPI.DTOs;
using TaskTrackerAPI.Models;
using TaskTrackerAPI.Services;

namespace TaskTrackerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController(ITaskItemService taskService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TaskItemDto>>> GetAll(
        [FromQuery] Guid? projectId,
        CancellationToken cancellationToken)
    {
        var tasks = await taskService.GetAllAsync(projectId, cancellationToken);
        return Ok(mapper.Map<IReadOnlyList<TaskItemDto>>(tasks));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskItemDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var task = await taskService.GetByIdAsync(id, cancellationToken);
        return task is null ? NotFound() : Ok(mapper.Map<TaskItemDto>(task));
    }

    [HttpPost]
    public async Task<ActionResult<TaskItemDto>> Create(CreateTaskItemDto request, CancellationToken cancellationToken)
    {
        var task = await taskService.CreateAsync(mapper.Map<TaskItem>(request), cancellationToken);
        if (task is null)
        {
            return NotFound($"Project '{request.ProjectId}' was not found.");
        }

        var response = mapper.Map<TaskItemDto>(task);
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, response);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TaskItemDto>> Update(Guid id, UpdateTaskItemDto request, CancellationToken cancellationToken)
    {
        var task = await taskService.UpdateAsync(id, mapper.Map<TaskItem>(request), cancellationToken);
        return task is null ? NotFound() : Ok(mapper.Map<TaskItemDto>(task));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        return await taskService.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
    }
}
