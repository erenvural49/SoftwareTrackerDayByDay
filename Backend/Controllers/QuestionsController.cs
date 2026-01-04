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
public class QuestionsController : ControllerBase
{
    private readonly AppDbContext _context;
    
    public QuestionsController(AppDbContext context)
    {
        _context = context;
    }
    
    private int GetUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    }
    
    [HttpPost]
    public async Task<ActionResult<QuestionResponse>> CreateQuestion(QuestionRequest request)
    {
        var userId = GetUserId();
        
        if (string.IsNullOrWhiteSpace(request.Content))
        {
            return BadRequest(new { message = "Soru içeriği boş olamaz." });
        }
        
        var question = new Question
        {
            UserId = userId,
            Content = request.Content
        };
        
        _context.Questions.Add(question);
        await _context.SaveChangesAsync();
        
        var user = await _context.Users.FindAsync(userId);
        
        return Ok(new QuestionResponse
        {
            Id = question.Id,
            UserName = $"{user?.FirstName} {user?.LastName}",
            Content = question.Content,
            CreatedAt = question.CreatedAt
        });
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<QuestionResponse>>> GetQuestions()
    {
        var questions = await _context.Questions
            .Include(q => q.User)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync();
        
        return Ok(questions.Select(q => new QuestionResponse
        {
            Id = q.Id,
            UserName = $"{q.User.FirstName} {q.User.LastName}",
            Content = q.Content,
            CreatedAt = q.CreatedAt
        }));
    }
}
