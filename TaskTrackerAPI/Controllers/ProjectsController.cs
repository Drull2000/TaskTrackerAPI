using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TaskTrackerAPI.DTOs;
using TaskTrackerAPI.Models;
using TaskTrackerAPI.Services;

namespace TaskTrackerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController(IProjectService projectService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProjectDto>>> GetAll(CancellationToken cancellationToken)
    {
        var projects = await projectService.GetAllAsync(cancellationToken);
        return Ok(mapper.Map<IReadOnlyList<ProjectDto>>(projects));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProjectDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var project = await projectService.GetByIdAsync(id, cancellationToken);
        return project is null ? NotFound() : Ok(mapper.Map<ProjectDto>(project));
    }

    [HttpPost]
    public async Task<ActionResult<ProjectDto>> Create(CreateProjectDto request, CancellationToken cancellationToken)
    {
        var project = await projectService.CreateAsync(mapper.Map<Project>(request), cancellationToken);
        var response = mapper.Map<ProjectDto>(project);
        return CreatedAtAction(nameof(GetById), new { id = project.Id }, response);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProjectDto>> Update(Guid id, UpdateProjectDto request, CancellationToken cancellationToken)
    {
        var project = await projectService.UpdateAsync(id, mapper.Map<Project>(request), cancellationToken);
        return project is null ? NotFound() : Ok(mapper.Map<ProjectDto>(project));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        return await projectService.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
    }
}
