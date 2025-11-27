using System.Linq.Expressions;

namespace TheBoard.Services.Models.Projects;

public class ProjectVM
{
    public int ProjectId { get; set; }
    public string Name { get; set; } = "";
    
    public static readonly Expression<Func<Domain.Entities.Project, ProjectVM>> ProjectionExpression =
        project => new ProjectVM
        {
            ProjectId = project.ProjectId,
            Name = project.Name
        };

    public ProjectVM()
    {
    }
    
    public ProjectVM(Domain.Entities.Project project)
    {
        ProjectId = project.ProjectId;
        Name = project.Name;
    }
}