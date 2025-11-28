using Microsoft.AspNetCore.Mvc;
using TheBoard.Application.Services;
using TheBoard.Services.Models.BoardItems;

namespace TheBoard.Controllers;

[ApiController]
[Route("v1/[controller]")]
public class BoardItemController : ControllerBase
{
    private readonly IBoardItemsService _boardItemsService;
    
    public BoardItemController(IBoardItemsService boardItemsService)
    {
        _boardItemsService = boardItemsService;
    }

    [HttpGet("BoardItems/{projectId}")]
    public async Task<ActionResult<List<BoardItemVM>>> GetBoardItemsByProjectIdAsync(int projectId)
    {
        var result = await _boardItemsService.GetBoardItemsByProjectIdAsync(projectId);
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<ActionResult<BoardItemVM>> CreateBoardItemAsync([FromBody] BoardItemCreateVM boardItemCreateVM)
    {
        var result = await _boardItemsService.CreateBoardItemAsync(boardItemCreateVM);
        return Ok(result);
    }

    [HttpPost("BatchUpdatePriority")]
    public async Task<ActionResult> UpdateBoardItemPriorityAsync([FromBody] List<BoardItemPriorityUpdateVM> updates)
    {
        try
        {
            await _boardItemsService.UpdateBoardItemPrioritiesAsync(updates);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpPost("MarkAsCompleted/{boardItemId}")]
    public async Task<ActionResult> MarkBoardItemAsCompletedAsync(int boardItemId) 
    {
        await _boardItemsService.MarkBoardItemAsCompletedAsync(boardItemId);
        return Ok();
    }

    [HttpDelete("{boardItemId}")]
    public async Task<ActionResult> DeleteBoardItemAsync(int boardItemId)
    {
        await _boardItemsService.DeleteBoardItemAsync(boardItemId);
        return Ok();
    }
}