namespace Weights.Domain.Entities;

public class WorkoutTemplate : Entity<int>
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsArchived { get; set; } = false;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<WorkoutTemplateExercise> WorkoutTemplateExercises { get; set; } = [];
    public ICollection<WorkoutSession> WorkoutSessions { get; set; } = [];
}
