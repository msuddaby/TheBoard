using System.Linq.Expressions;

namespace TheBoard.Services.Models.BoardItems;

public class BoardItemVM
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public int ProjectId { get; set; }
    public int Priority { get; set; }
    public bool IsCompleted { get; set; }
    
    public static readonly Expression<Func<Domain.Entities.BoardItem, BoardItemVM>> ProjectionExpression =
        boardItem => new BoardItemVM
        {
            Id = boardItem.Id,
            Title = boardItem.Title,
            Description = boardItem.Description,
            CreatedAt = boardItem.CreatedAt,
            ProjectId = boardItem.ProjectId,
            Priority = boardItem.Priority,
            IsCompleted = boardItem.IsCompleted
        };

    public BoardItemVM()
    {
    }
    
    public BoardItemVM(Domain.Entities.BoardItem boardItem)
    {
        Id = boardItem.Id;
        Title = boardItem.Title;
        Description = boardItem.Description;
        CreatedAt = boardItem.CreatedAt;
        ProjectId = boardItem.ProjectId;
        Priority = boardItem.Priority;
        IsCompleted = boardItem.IsCompleted;
    }
}