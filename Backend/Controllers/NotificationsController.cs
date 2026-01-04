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
public class NotificationsController : ControllerBase
{
    private readonly AppDbContext _context;
    
    public NotificationsController(AppDbContext context)
    {
        _context = context;
    }
    
    private int GetUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<NotificationResponse>>> GetNotifications()
    {
        var userId = GetUserId();
        
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
        
        return Ok(notifications.Select(n => new NotificationResponse
        {
            Id = n.Id,
            Message = n.Message,
            IsRead = n.IsRead,
            CreatedAt = n.CreatedAt
        }));
    }
    
    [HttpPost("{id}/mark-read")]
    public async Task<ActionResult> MarkAsRead(int id)
    {
        var userId = GetUserId();
        
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);
        
        if (notification == null)
        {
            return NotFound(new { message = "Bildirim bulunamadı." });
        }
        
        notification.IsRead = true;
        await _context.SaveChangesAsync();
        
        return Ok();
    }
}
