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
[Authorize]
public class DailyReportsController : ControllerBase
{
    private readonly AppDbContext _context;
    
    public DailyReportsController(AppDbContext context)
    {
        _context = context;
    }
    
    private int GetUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    }
    
    private string GetUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value ?? "";
    }
    
    [HttpPost]
    public async Task<ActionResult<DailyReportResponse>> CreateDailyReport(DailyReportRequest request)
    {
        var userId = GetUserId();
        var today = DateTime.UtcNow.Date;
        
        // Check if report already exists for today
        var existingReport = await _context.DailyReports
            .FirstOrDefaultAsync(dr => dr.UserId == userId && dr.Date.Date == today);
        
        if (existingReport != null)
        {
            return BadRequest(new { message = "Bugün için zaten bir rapor doldurdunuz." });
        }
        
        // Validate all fields are filled
        if (string.IsNullOrWhiteSpace(request.WhatILearned) ||
            string.IsNullOrWhiteSpace(request.WhereIStruggled) ||
            string.IsNullOrWhiteSpace(request.ErrorsSolved) ||
            string.IsNullOrWhiteSpace(request.TomorrowGoal) ||
            string.IsNullOrWhiteSpace(request.EffortLevel))
        {
            return BadRequest(new { message = "Tüm alanları doldurmalısınız." });
        }
        
        var report = new DailyReport
        {
            UserId = userId,
            Date = today,
            WhatILearned = request.WhatILearned,
            WhereiStruggled = request.WhereIStruggled,
            ErrorsSolved = request.ErrorsSolved,
            TomorrowGoal = request.TomorrowGoal,
            EffortLevel = request.EffortLevel
        };
        
        _context.DailyReports.Add(report);
        await _context.SaveChangesAsync();
        
        var user = await _context.Users.FindAsync(userId);
        
        return Ok(new DailyReportResponse
        {
            Id = report.Id,
            UserId = report.UserId,
            UserName = $"{user?.FirstName} {user?.LastName}",
            Date = report.Date,
            WhatILearned = report.WhatILearned,
            WhereIStruggled = report.WhereiStruggled,
            ErrorsSolved = report.ErrorsSolved,
            TomorrowGoal = report.TomorrowGoal,
            EffortLevel = report.EffortLevel
        });
    }
    
    [HttpGet("my-reports")]
    public async Task<ActionResult<IEnumerable<DailyReportResponse>>> GetMyReports()
    {
        var userId = GetUserId();
        
        var reports = await _context.DailyReports
            .Where(dr => dr.UserId == userId)
            .Include(dr => dr.User)
            .Include(dr => dr.Feedbacks)
                .ThenInclude(f => f.Mentor)
            .OrderByDescending(dr => dr.Date)
            .ToListAsync();
        
        return Ok(reports.Select(r => new DailyReportResponse
        {
            Id = r.Id,
            UserId = r.UserId,
            UserName = $"{r.User.FirstName} {r.User.LastName}",
            Date = r.Date,
            WhatILearned = r.WhatILearned,
            WhereIStruggled = r.WhereiStruggled,
            ErrorsSolved = r.ErrorsSolved,
            TomorrowGoal = r.TomorrowGoal,
            EffortLevel = r.EffortLevel,
            Feedbacks = r.Feedbacks.Select(f => new FeedbackResponse
            {
                Id = f.Id,
                MentorName = $"{f.Mentor.FirstName} {f.Mentor.LastName}",
                FeedbackType = f.FeedbackType,
                CreatedAt = f.CreatedAt
            }).ToList()
        }));
    }
    
    [HttpGet("all-reports")]
    [Authorize(Roles = "Mentor")]
    public async Task<ActionResult<IEnumerable<DailyReportResponse>>> GetAllReports()
    {
        var reports = await _context.DailyReports
            .Include(dr => dr.User)
            .Include(dr => dr.Feedbacks)
                .ThenInclude(f => f.Mentor)
            .OrderByDescending(dr => dr.Date)
            .ToListAsync();
        
        return Ok(reports.Select(r => new DailyReportResponse
        {
            Id = r.Id,
            UserId = r.UserId,
            UserName = $"{r.User.FirstName} {r.User.LastName}",
            Date = r.Date,
            WhatILearned = r.WhatILearned,
            WhereIStruggled = r.WhereiStruggled,
            ErrorsSolved = r.ErrorsSolved,
            TomorrowGoal = r.TomorrowGoal,
            EffortLevel = r.EffortLevel,
            Feedbacks = r.Feedbacks.Select(f => new FeedbackResponse
            {
                Id = f.Id,
                MentorName = $"{f.Mentor.FirstName} {f.Mentor.LastName}",
                FeedbackType = f.FeedbackType,
                CreatedAt = f.CreatedAt
            }).ToList()
        }));
    }
    
    [HttpGet("contributions/{userId?}")]
    public async Task<ActionResult<IEnumerable<ContributionData>>> GetContributions(int? userId = null)
    {
        var targetUserId = userId ?? GetUserId();
        var role = GetUserRole();
        
        // Only mentors can view other users' contributions
        if (targetUserId != GetUserId() && role != "Mentor")
        {
            return Forbid();
        }
        
        var startDate = new DateTime(2026, 1, 26);
        var endDate = new DateTime(2026, 2, 16);
        
        var reports = await _context.DailyReports
            .Where(dr => dr.UserId == targetUserId && dr.Date >= startDate && dr.Date <= endDate)
            .ToListAsync();
        
        var contributions = new List<ContributionData>();
        
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            var report = reports.FirstOrDefault(r => r.Date.Date == date.Date);
            var completionLevel = 0;
            
            if (report != null)
            {
                // Count how many fields are filled
                var filledFields = 0;
                if (!string.IsNullOrWhiteSpace(report.WhatILearned)) filledFields++;
                if (!string.IsNullOrWhiteSpace(report.WhereiStruggled)) filledFields++;
                if (!string.IsNullOrWhiteSpace(report.ErrorsSolved)) filledFields++;
                if (!string.IsNullOrWhiteSpace(report.TomorrowGoal)) filledFields++;
                if (!string.IsNullOrWhiteSpace(report.EffortLevel)) filledFields++;
                
                completionLevel = filledFields;
            }
            
            contributions.Add(new ContributionData
            {
                Date = date,
                CompletionLevel = completionLevel
            });
        }
        
        return Ok(contributions);
    }
}
