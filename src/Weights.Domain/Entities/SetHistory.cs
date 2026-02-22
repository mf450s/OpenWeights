using Weights.Domain.Enums;

namespace Weights.Domain.Entities;

public class SetHistory : Entity<int>
{
    public int WorkoutSessionId { get; set; }
    public WorkoutSession WorkoutSession { get; set; } = null!;

    public int ExerciseId { get; set; }
    public Exercise Exercise { get; set; } = null!;

    public int? WorkoutTemplateExerciseId { get; set; }
    public WorkoutTemplateExercise? WorkoutTemplateExercise { get; set; }

    public int SetNumber { get; set; }
    public decimal? Weight { get; set; }
    public int? Reps { get; set; }
    public decimal? RIR { get; set; }
    public int? DurationSeconds { get; set; }
    public decimal? DistanceMeters { get; set; }
    public Side? Side { get; set; }
    public DateTime PerformedAt { get; set; }
}
