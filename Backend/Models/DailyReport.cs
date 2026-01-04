namespace Backend.Models;

public class DailyReport
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime Date { get; set; }
    public string WhatILearned { get; set; } = string.Empty; // Question 1
    public string WhereiStruggled { get; set; } = string.Empty; // Question 2
    public string ErrorsSolved { get; set; } = string.Empty; // Question 3
    public string TomorrowGoal { get; set; } = string.Empty; // Question 4
    public string EffortLevel { get; set; } = string.Empty; // "Güzel", "Orta", "Kötü"
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}
