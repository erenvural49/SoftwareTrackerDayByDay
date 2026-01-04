using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<DailyReport> DailyReports { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<WeeklyCard> WeeklyCards { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // User configuration
        modelBuilder.Entity<User>()
            .HasIndex(u => new { u.FirstName, u.LastName })
            .IsUnique();
        
        // DailyReport configuration
        modelBuilder.Entity<DailyReport>()
            .HasOne(dr => dr.User)
            .WithMany(u => u.DailyReports)
            .HasForeignKey(dr => dr.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Feedback configuration
        modelBuilder.Entity<Feedback>()
            .HasOne(f => f.DailyReport)
            .WithMany(dr => dr.Feedbacks)
            .HasForeignKey(f => f.DailyReportId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Feedback>()
            .HasOne(f => f.Mentor)
            .WithMany(u => u.GivenFeedbacks)
            .HasForeignKey(f => f.MentorId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Question configuration
        modelBuilder.Entity<Question>()
            .HasOne(q => q.User)
            .WithMany(u => u.Questions)
            .HasForeignKey(q => q.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // WeeklyCard configuration
        modelBuilder.Entity<WeeklyCard>()
            .HasOne(wc => wc.User)
            .WithMany(u => u.WeeklyCards)
            .HasForeignKey(wc => wc.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Notification configuration
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
