namespace TheBoard.Domain.Entities;

public class Project
{
    public int ProjectId { get; set; }
    public string Name { get; set; }
    public virtual ICollection<BoardItem> BoardItems { get; set; }
}