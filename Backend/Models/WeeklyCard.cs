namespace Backend.Models;

public class WeeklyCard
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int WeekNumber { get; set; }
    public int DaysWorked { get; set; }
    public int TopicsLearned { get; set; }
    public int ErrorsSolved { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public User User { get; set; } = null!;
}
