using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Backend.Services;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly AuthService _authService;
    
    public AuthController(AppDbContext context, AuthService authService)
    {
        _context = context;
        _authService = authService;
    }
    
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
        {
            return BadRequest(new { message = "Ad ve soyad gereklidir." });
        }
        
        // Check if user already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.FirstName == request.FirstName && u.LastName == request.LastName);
        
        if (existingUser != null)
        {
            return BadRequest(new { message = "Bu ad ve soyad ile kayıtlı kullanıcı zaten mevcut." });
        }
        
        // Determine role
        var role = _authService.IsMentor(request.FirstName, request.LastName) ? "Mentor" : "Participant";
        
        // Create new user
        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = role
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        // Generate token
        var token = _authService.GenerateToken(user.Id, user.FirstName, user.LastName, user.Role);
        
        return Ok(new AuthResponse
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role,
            Token = token
        });
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
        {
            return BadRequest(new { message = "Ad ve soyad gereklidir." });
        }
        
        // Find user
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.FirstName == request.FirstName && u.LastName == request.LastName);
        
        if (user == null)
        {
            return NotFound(new { message = "Kullanıcı bulunamadı." });
        }
        
        // Generate token
        var token = _authService.GenerateToken(user.Id, user.FirstName, user.LastName, user.Role);
        
        return Ok(new AuthResponse
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role,
            Token = token
        });
    }
}
