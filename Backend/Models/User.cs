namespace Backend.Models;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = "Participant"; // "Mentor" or "Participant"
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<DailyReport> DailyReports { get; set; } = new List<DailyReport>();
    public ICollection<Feedback> GivenFeedbacks { get; set; } = new List<Feedback>();
    public ICollection<Question> Questions { get; set; } = new List<Question>();
    public ICollection<WeeklyCard> WeeklyCards { get; set; } = new List<WeeklyCard>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
