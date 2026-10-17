using System.ComponentModel.DataAnnotations;
using TaskTrackerAPI.Models;

namespace TaskTrackerAPI.DTOs;

public class TaskItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskPriority Priority { get; set; }
    public TaskTrackerAPI.Models.TaskStatus Status { get; set; }
    public Guid ProjectId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateTaskItemDto
{
    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }
    public TaskPriority Priority { get; set; }
    public TaskTrackerAPI.Models.TaskStatus Status { get; set; }
    public Guid ProjectId { get; set; }
}

public class UpdateTaskItemDto
{
    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }
    public TaskPriority Priority { get; set; }
    public TaskTrackerAPI.Models.TaskStatus Status { get; set; }
    public Guid ProjectId { get; set; }
}
