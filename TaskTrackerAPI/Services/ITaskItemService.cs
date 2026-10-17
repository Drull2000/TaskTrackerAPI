using TaskTrackerAPI.Models;

namespace TaskTrackerAPI.Services;

public interface ITaskItemService
{
    Task<IReadOnlyList<TaskItem>> GetAllAsync(Guid? projectId = null, CancellationToken cancellationToken = default);
    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TaskItem?> CreateAsync(TaskItem task, CancellationToken cancellationToken = default);
    Task<TaskItem?> UpdateAsync(Guid id, TaskItem task, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
