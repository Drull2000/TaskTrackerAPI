using Microsoft.EntityFrameworkCore;
using TaskTrackerAPI.Data;
using TaskTrackerAPI.Models;

namespace TaskTrackerAPI.Services;

public class TaskItemService(TaskTrackerDbContext dbContext) : ITaskItemService
{
    public async Task<IReadOnlyList<TaskItem>> GetAllAsync(Guid? projectId = null, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Tasks
            .AsNoTracking()
            .Where(task => !task.IsDeleted && !task.Project.IsDeleted);

        if (projectId.HasValue)
        {
            query = query.Where(task => task.ProjectId == projectId.Value);
        }

        return await query.OrderBy(task => task.CreatedAt).ToListAsync(cancellationToken);
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.Tasks
            .AsNoTracking()
            .SingleOrDefaultAsync(task => task.Id == id && !task.IsDeleted && !task.Project.IsDeleted, cancellationToken);

    public async Task<TaskItem?> CreateAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        var projectExists = await dbContext.Projects
            .AnyAsync(project => project.Id == task.ProjectId && !project.IsDeleted, cancellationToken);
        if (!projectExists)
        {
            return null;
        }

        task.Id = Guid.NewGuid();
        task.CreatedAt = DateTime.UtcNow;
        task.IsDeleted = false;
        dbContext.Tasks.Add(task);
        await dbContext.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(task.Id, cancellationToken);
    }

    public async Task<TaskItem?> UpdateAsync(Guid id, TaskItem task, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Tasks
            .Include(item => item.Project)
            .SingleOrDefaultAsync(item => item.Id == id && !item.IsDeleted && !item.Project.IsDeleted, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        var projectExists = await dbContext.Projects
            .AnyAsync(project => project.Id == task.ProjectId && !project.IsDeleted, cancellationToken);
        if (!projectExists)
        {
            return null;
        }

        existing.Title = task.Title;
        existing.Description = task.Description;
        existing.Priority = task.Priority;
        existing.Status = task.Status;
        existing.ProjectId = task.ProjectId;
        await dbContext.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var task = await dbContext.Tasks
            .SingleOrDefaultAsync(item => item.Id == id && !item.IsDeleted && !item.Project.IsDeleted, cancellationToken);
        if (task is null)
        {
            return false;
        }

        task.IsDeleted = true;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
