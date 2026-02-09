namespace Weights.Domain.Entities;

public class WorkoutTemplateExercise : Entity<int>
{
    public int WorkoutTemplateId { get; set; }
    public WorkoutTemplate WorkoutTemplate { get; set; } = null!;

    public int ExerciseId { get; set; }
    public Exercise Exercise { get; set; } = null!;

    public int OrderIndex { get; set; }
    public int TargetSets { get; set; }
    public string TargetReps { get; set; } = string.Empty;
    public decimal? TargetRPE { get; set; }
    public int? RestSeconds { get; set; }
}
