namespace Weights.Application.DTOs.Sessions;

public class SessionDetailResponse
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int? WorkoutTemplateId { get; set; }
    public DateTime Date { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? Note { get; set; }
    public List<SessionSetResponse> Sets { get; set; } = new();
}

public class SessionSetResponse
{
    public int Id { get; set; }
    public int ExerciseId { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
    public int SetNumber { get; set; }
    public decimal? Weight { get; set; }
    public int? Reps { get; set; }
    public decimal? RIR { get; set; }
    public int? DurationSeconds { get; set; }
    public decimal? DistanceMeters { get; set; }
    public string? Side { get; set; }
    public int? WorkoutTemplateExerciseId { get; set; }
    public DateTime PerformedAt { get; set; }
}
