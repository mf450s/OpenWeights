using System.ComponentModel.DataAnnotations;

namespace Weights.Application.DTOs.Sessions;

public class CreateSessionRequest
{
    public int? WorkoutTemplateId { get; set; }

    [StringLength(100)]
    public string? Name { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public string? Note { get; set; }

    [Required]
    public List<SetDto> Sets { get; set; } = new();
}

public class SetDto
{
    [Required]
    public int ExerciseId { get; set; }

    [Required]
    public int SetNumber { get; set; }

    public decimal? Weight { get; set; }

    public int? Reps { get; set; }

    [Range(0, 10)]
    public decimal? Rir { get; set; }

    public int? DurationSeconds { get; set; }

    public decimal? DistanceMeters { get; set; }

    [Required]
    public DateTime PerformedAt { get; set; }
}
