using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class WeeklyCardService
{
    private readonly AppDbContext _context;
    
    public WeeklyCardService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task GenerateWeeklyCardsAsync()
    {
        // Get all participants
        var participants = await _context.Users
            .Where(u => u.Role == "Participant")
            .ToListAsync();
        
        // Determine current week
        var now = DateTime.UtcNow;
        var startDate = new DateTime(2026, 1, 26);
        var weekNumber = (int)Math.Ceiling((now - startDate).TotalDays / 7.0);
        
        if (weekNumber < 1) weekNumber = 1;
        if (weekNumber > 3) weekNumber = 3;
        
        // Calculate week start and end
        var weekStart = startDate.AddDays((weekNumber - 1) * 7);
        var weekEnd = weekStart.AddDays(6);
        if (weekEnd > new DateTime(2026, 2, 16)) weekEnd = new DateTime(2026, 2, 16);
        
        foreach (var participant in participants)
        {
            // Check if card already exists for this week
            var existingCard = await _context.WeeklyCards
                .FirstOrDefaultAsync(wc => wc.UserId == participant.Id && wc.WeekNumber == weekNumber);
            
            if (existingCard != null) continue;
            
            // Get reports for this week
            var reports = await _context.DailyReports
                .Where(dr => dr.UserId == participant.Id && 
                            dr.Date >= weekStart && 
                            dr.Date <= weekEnd)
                .ToListAsync();
            
            var daysWorked = reports.Count;
            var topicsLearned = reports.Count(r => !string.IsNullOrWhiteSpace(r.WhatILearned));
            var errorsSolved = reports.Count(r => !string.IsNullOrWhiteSpace(r.ErrorsSolved) && 
                                                   r.ErrorsSolved.ToLower() != "yok" && 
                                                   r.ErrorsSolved.ToLower() != "yoktu");
            
            var message = $"Bu hafta {daysWorked} gün çalıştın, toplam {topicsLearned} yeni konu öğrendin ve {errorsSolved} hata çözdün. C# temellerinde harika bir temel attın!";
            
            var card = new WeeklyCard
            {
                UserId = participant.Id,
                WeekNumber = weekNumber,
                DaysWorked = daysWorked,
                TopicsLearned = topicsLearned,
                ErrorsSolved = errorsSolved,
                Message = message
            };
            
            _context.WeeklyCards.Add(card);
        }
        
        await _context.SaveChangesAsync();
    }
}
