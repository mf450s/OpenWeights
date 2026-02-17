using Weights.Domain.Enums;

namespace Weights.Domain.Entities;

public class Exercise : Entity<int>
{
    public string Name { get; set; } = string.Empty;
    public TrackType TrackType { get; set; }
    public string? Description { get; set; }
    public Laterality Laterality { get; set; } = Laterality.Bilateral;

    // Navigation properties
    public ICollection<ExerciseMuscle> ExerciseMuscles { get; set; } = new List<ExerciseMuscle>();
    public ICollection<WorkoutTemplateExercise> WorkoutTemplateExercises { get; set; } = new List<WorkoutTemplateExercise>();
    public ICollection<SetHistory> SetHistories { get; set; } = new List<SetHistory>();
}
