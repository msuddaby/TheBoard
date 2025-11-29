namespace TheBoard.Domain.Entities;

public class BoardItem
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public int ProjectId { get; set; }
    public virtual Project Project { get; set; } = null!;
    public int Priority { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
}