namespace Backend.DTOs;

public class RegisterRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

public class LoginRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

public class AuthResponse
{
    public int UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}

public class DailyReportRequest
{
    public string WhatILearned { get; set; } = string.Empty;
    public string WhereIStruggled { get; set; } = string.Empty;
    public string ErrorsSolved { get; set; } = string.Empty;
    public string TomorrowGoal { get; set; } = string.Empty;
    public string EffortLevel { get; set; } = string.Empty;
}

public class DailyReportResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string WhatILearned { get; set; } = string.Empty;
    public string WhereIStruggled { get; set; } = string.Empty;
    public string ErrorsSolved { get; set; } = string.Empty;
    public string TomorrowGoal { get; set; } = string.Empty;
    public string EffortLevel { get; set; } = string.Empty;
    public List<FeedbackResponse> Feedbacks { get; set; } = new List<FeedbackResponse>();
}

public class FeedbackRequest
{
    public int DailyReportId { get; set; }
    public string FeedbackType { get; set; } = string.Empty; // "Görüldü" or "Tebrikler"
}

public class FeedbackResponse
{
    public int Id { get; set; }
    public string MentorName { get; set; } = string.Empty;
    public string FeedbackType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class QuestionRequest
{
    public string Content { get; set; } = string.Empty;
}

public class QuestionResponse
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class ContributionData
{
    public DateTime Date { get; set; }
    public int CompletionLevel { get; set; } // 0-5
}

public class NotificationResponse
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class WeeklyCardResponse
{
    public int Id { get; set; }
    public int WeekNumber { get; set; }
    public int DaysWorked { get; set; }
    public int TopicsLearned { get; set; }
    public int ErrorsSolved { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
