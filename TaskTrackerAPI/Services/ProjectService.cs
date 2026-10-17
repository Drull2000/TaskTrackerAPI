using Microsoft.EntityFrameworkCore;
using TaskTrackerAPI.Data;
using TaskTrackerAPI.Models;

namespace TaskTrackerAPI.Services;

public class ProjectService(TaskTrackerDbContext dbContext) : IProjectService
{
    public async Task<IReadOnlyList<Project>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Projects
            .AsNoTracking()
            .Include(project => project.Tasks.Where(task => !task.IsDeleted))
            .Where(project => !project.IsDeleted)
            .OrderBy(project => project.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.Projects
            .AsNoTracking()
            .Include(project => project.Tasks.Where(task => !task.IsDeleted))
            .SingleOrDefaultAsync(project => project.Id == id && !project.IsDeleted, cancellationToken);

    public async Task<Project> CreateAsync(Project project, CancellationToken cancellationToken = default)
    {
        project.Id = Guid.NewGuid();
        project.CreatedAt = DateTime.UtcNow;
        project.IsDeleted = false;
        dbContext.Projects.Add(project);
        await dbContext.SaveChangesAsync(cancellationToken);
        return project;
    }

    public async Task<Project?> UpdateAsync(Guid id, Project project, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Projects
            .SingleOrDefaultAsync(item => item.Id == id && !item.IsDeleted, cancellationToken);
        if (existing is null)
        {
            return null;
        }

        existing.Name = project.Name;
        existing.Description = project.Description;
        await dbContext.SaveChangesAsync(cancellationToken);
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var project = await dbContext.Projects
            .Include(item => item.Tasks)
            .SingleOrDefaultAsync(item => item.Id == id && !item.IsDeleted, cancellationToken);
        if (project is null)
        {
            return false;
        }

        project.IsDeleted = true;
        foreach (var task in project.Tasks)
        {
            task.IsDeleted = true;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
