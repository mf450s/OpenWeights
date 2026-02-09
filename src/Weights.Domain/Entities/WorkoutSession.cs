namespace Weights.Domain.Entities;

public class WorkoutSession : Entity<int>
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public int? WorkoutTemplateId { get; set; }
    public WorkoutTemplate? WorkoutTemplate { get; set; }

    public string? Name { get; set; }
    public string? Note { get; set; }
    public DateTime Date { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    // Navigation properties
    public ICollection<SetHistory> SetHistories { get; set; } = new List<SetHistory>();
}
