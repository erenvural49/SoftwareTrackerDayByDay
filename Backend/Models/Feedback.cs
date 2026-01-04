namespace Backend.Models;

public class Feedback
{
    public int Id { get; set; }
    public int DailyReportId { get; set; }
    public int MentorId { get; set; }
    public string FeedbackType { get; set; } = string.Empty; // "Görüldü" or "Tebrikler"
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public DailyReport DailyReport { get; set; } = null!;
    public User Mentor { get; set; } = null!;
}
