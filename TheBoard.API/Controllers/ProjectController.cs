using Microsoft.AspNetCore.Mvc;
using TheBoard.Application.Services;
using TheBoard.Services.Models.Projects;

namespace TheBoard.Controllers;

[ApiController]
[Route("v1/[controller]")]
public class ProjectController : ControllerBase
{
    private readonly IProjectsService _projectsService;

    public ProjectController(IProjectsService projectsService)
    {
        _projectsService = projectsService;
    }

    [HttpGet("Projects")]
    public async Task<ActionResult<List<ProjectVM>>> GetAllProjectsAsync()
    {
        var result = await _projectsService.GetAllProjectsAsync();
        return Ok(result);
    }
    
    [HttpGet("{ProjectId}")]
    public async Task<ActionResult<ProjectVM>> GetProjectByIdAsync(int ProjectId)
    {
        var result = await _projectsService.GetProjectByIdAsync(ProjectId);
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<ActionResult<ProjectVM>> CreateProjectAsync([FromBody] string name)
    {
        var result = await _projectsService.CreateProjectAsync(name);
        return Ok(result);
    }
}