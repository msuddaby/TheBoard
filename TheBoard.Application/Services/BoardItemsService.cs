using Microsoft.EntityFrameworkCore;
using TheBoard.Infrastructure.Data;
using TheBoard.Services.Models.BoardItems;

namespace TheBoard.Application.Services;

public interface IBoardItemsService
{
    Task<List<BoardItemVM>> GetBoardItemsByProjectIdAsync(int projectId);
    Task<BoardItemVM> CreateBoardItemAsync(BoardItemCreateVM boardItemCreateVM);
    Task UpdateBoardItemPrioritiesAsync(List<BoardItemPriorityUpdateVM> updates);
    Task MarkBoardItemAsCompletedAsync(int boardItemId);
    Task DeleteBoardItemAsync(int boardItemId);
    Task<BoardItemVM> UpdateBoardItemAsync(BoardItemCreateVM boardItemUpdateVM);
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
            .Where(bi => bi.ProjectId == projectId && !bi.IsCompleted)
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

    public async Task<BoardItemVM> UpdateBoardItemAsync(BoardItemCreateVM boardItemUpdateVM)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        var boardItem = await dbContext.BoardItems.FindAsync(boardItemUpdateVM.Id);
        if (boardItem == null)
        {
            throw new Exception("Board item not found");
        }
        boardItem.Title = boardItemUpdateVM.Title;
        boardItem.Description = boardItemUpdateVM.Description;
        boardItem.Priority = boardItemUpdateVM.Priority;
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
    
    public async Task MarkBoardItemAsCompletedAsync(int boardItemId)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        
        var boardItem = await dbContext.BoardItems.FindAsync(boardItemId);
        if (boardItem == null)
        {
            throw new Exception("Board item not found");
        }
        
        boardItem.IsCompleted = true;
        boardItem.CompletedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync();
    }
    
    public async Task DeleteBoardItemAsync(int boardItemId)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
        
        var boardItem = await dbContext.BoardItems.FindAsync(boardItemId);
        if (boardItem == null)
        {
            throw new Exception("Board item not found");
        }
        
        dbContext.BoardItems.Remove(boardItem);
        await dbContext.SaveChangesAsync();
    }
}