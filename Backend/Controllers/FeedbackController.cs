using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using System.Security.Claims;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Mentor")]
public class FeedbackController : ControllerBase
{
    private readonly AppDbContext _context;
    
    public FeedbackController(AppDbContext context)
    {
        _context = context;
    }
    
    private int GetUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    }
    
    [HttpPost]
    public async Task<ActionResult<FeedbackResponse>> CreateFeedback(FeedbackRequest request)
    {
        var mentorId = GetUserId();
        
        // Validate feedback type
        if (request.FeedbackType != "Görüldü" && request.FeedbackType != "Tebrikler")
        {
            return BadRequest(new { message = "Geçersiz geri bildirim türü." });
        }
        
        // Check if daily report exists
        var report = await _context.DailyReports
            .Include(dr => dr.User)
            .FirstOrDefaultAsync(dr => dr.Id == request.DailyReportId);
        
        if (report == null)
        {
            return NotFound(new { message = "Günlük rapor bulunamadı." });
        }
        
        // Create feedback
        var feedback = new Feedback
        {
            DailyReportId = request.DailyReportId,
            MentorId = mentorId,
            FeedbackType = request.FeedbackType
        };
        
        _context.Feedbacks.Add(feedback);
        
        // If feedback is "Tebrikler", create a notification for the participant
        if (request.FeedbackType == "Tebrikler")
        {
            var notification = new Notification
            {
                UserId = report.UserId,
                Message = "Mentorun dünkü çalışmanı tebrik ediyor"
            };
            
            _context.Notifications.Add(notification);
        }
        
        await _context.SaveChangesAsync();
        
        var mentor = await _context.Users.FindAsync(mentorId);
        
        return Ok(new FeedbackResponse
        {
            Id = feedback.Id,
            MentorName = $"{mentor?.FirstName} {mentor?.LastName}",
            FeedbackType = feedback.FeedbackType,
            CreatedAt = feedback.CreatedAt
        });
    }
}
