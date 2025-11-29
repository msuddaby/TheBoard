using TheBoard.Domain.Entities;

namespace TheBoard.Services.Models.BoardItems;

public class BoardItemCreateVM
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ProjectId { get; set; }
    public int Priority { get; set; }

    public BoardItem ToEntity()
    {
        return new BoardItem()
        {
            Id = this.Id,
            CreatedAt = DateTime.UtcNow,
            Title = this.Title,
            Description = this.Description,
            ProjectId = this.ProjectId,
            Priority = this.Priority,
            IsCompleted = false
        };
    }
}