namespace Weights.Domain.Entities;

public class User : Entity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<WorkoutTemplate> WorkoutTemplates { get; set; } = new List<WorkoutTemplate>();
    public ICollection<WorkoutSession> WorkoutSessions { get; set; } = new List<WorkoutSession>();
}
