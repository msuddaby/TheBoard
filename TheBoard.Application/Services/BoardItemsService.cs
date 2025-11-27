using Microsoft.EntityFrameworkCore;
using TheBoard.Infrastructure.Data;
using TheBoard.Services.Models.BoardItems;

namespace TheBoard.Application.Services;

public interface IBoardItemsService
{
    Task<List<BoardItemVM>> GetBoardItemsByProjectIdAsync(int projectId);
    Task<BoardItemVM> CreateBoardItemAsync(BoardItemCreateVM boardItemCreateVM);
    Task UpdateBoardItemPrioritiesAsync(List<BoardItemPriorityUpdateVM> updates);
}

public class BoardItemsService : IBoardItemsService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    
    public BoardItemsService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }
    
    public async Task<List<BoardItemVM>> GetBoardItemsByProjectIdAsync(int projectId)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var boardItems = await dbContext.BoardItems
            .Where(bi => bi.ProjectId == projectId)
            .ToListAsync();
        
        return boardItems.Select(bi => new BoardItemVM(bi)).ToList();
    }
    
    public async Task<BoardItemVM> CreateBoardItemAsync(BoardItemCreateVM boardItemCreateVM)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        
        var boardItem = boardItemCreateVM.ToEntity();
        
        dbContext.BoardItems.Add(boardItem);
        await dbContext.SaveChangesAsync();
        
        return new BoardItemVM(boardItem);
    }
    
    public async Task UpdateBoardItemPrioritiesAsync(List<BoardItemPriorityUpdateVM> updates)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
    
        var itemIds = updates.Select(u => u.BoardItemId).ToList();
        var boardItems = await dbContext.BoardItems
            .Where(bi => itemIds.Contains(bi.Id))
            .ToListAsync();
    
        if (boardItems.Count != updates.Count)
        {
            throw new Exception("One or more board items not found");
        }
    
        foreach (var update in updates)
        {
            var item = boardItems.First(bi => bi.Id == update.BoardItemId);
            item.Priority = update.NewPriority;
        }
    
        await dbContext.SaveChangesAsync();
    }
}