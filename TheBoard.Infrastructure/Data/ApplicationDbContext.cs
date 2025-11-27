using Microsoft.EntityFrameworkCore;
using TheBoard.Domain.Entities;

namespace TheBoard.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
    
    public DbSet<Project> Projects { get; set; }
    public DbSet<BoardItem> BoardItems { get; set; }
}