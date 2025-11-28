
using Microsoft.EntityFrameworkCore;
using TheBoard.Infrastructure.Data;
using TheBoard.Services.Models.Projects;

namespace TheBoard.Application.Services;

public interface IProjectsService
{
    Task<List<ProjectVM>> GetAllProjectsAsync();
    Task<ProjectVM> CreateProjectAsync(string name);
    Task<ProjectVM?> GetProjectByIdAsync(int projectId);
}

public class ProjectsService : IProjectsService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    
    public ProjectsService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }
    
    public async Task<List<ProjectVM>> GetAllProjectsAsync()
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var projects = await dbContext.Projects.ToListAsync();
        
        return projects.Select(p => new ProjectVM(p)).ToList();
    }
    
    public async Task<ProjectVM?> GetProjectByIdAsync(int projectId)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var project = await dbContext.Projects.FindAsync(projectId);
        
        if (project == null)
        {
            return null;
        }
        
        return new ProjectVM(project);
    }
    
    public async Task<ProjectVM> CreateProjectAsync(string name)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        
        var project = new Domain.Entities.Project
        {
            Name = name
        };
        
        dbContext.Projects.Add(project);
        await dbContext.SaveChangesAsync();
        
        return new ProjectVM(project);
    }
    
    
}