namespace Weights.Application.DTOs.Exercises;

public class ExerciseHistoryResponse
{
    public int ExerciseId { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
    public List<ExerciseHistorySetGroup> Sessions { get; set; } = new();
}

public class ExerciseHistorySetGroup
{
    public int SessionId { get; set; }
    public DateTime Date { get; set; }
    public List<ExerciseHistorySet> Sets { get; set; } = new();
}

public class ExerciseHistorySet
{
    public int SetNumber { get; set; }
    public decimal? Weight { get; set; }
    public int? Reps { get; set; }
    public decimal? RIR { get; set; }
    public int? DurationSeconds { get; set; }
    public decimal? DistanceMeters { get; set; }
}
