using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.DTOs;
using System.Security.Claims;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WeeklyCardsController : ControllerBase
{
    private readonly AppDbContext _context;
    
    public WeeklyCardsController(AppDbContext context)
    {
        _context = context;
    }
    
    private int GetUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WeeklyCardResponse>>> GetWeeklyCards()
    {
        var userId = GetUserId();
        
        var cards = await _context.WeeklyCards
            .Where(wc => wc.UserId == userId)
            .OrderByDescending(wc => wc.WeekNumber)
            .ToListAsync();
        
        return Ok(cards.Select(c => new WeeklyCardResponse
        {
            Id = c.Id,
            WeekNumber = c.WeekNumber,
            DaysWorked = c.DaysWorked,
            TopicsLearned = c.TopicsLearned,
            ErrorsSolved = c.ErrorsSolved,
            Message = c.Message,
            CreatedAt = c.CreatedAt
        }));
    }
}
