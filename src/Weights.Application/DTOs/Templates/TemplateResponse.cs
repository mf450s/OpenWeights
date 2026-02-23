namespace Weights.Application.DTOs.Templates;

public class TemplateResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsArchived { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<TemplateExerciseResponse> Exercises { get; set; } = [];
}

public class TemplateExerciseResponse
{
    public int Id { get; set; }
    public int ExerciseId { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public int TargetSets { get; set; }
    public int? TargetRepsMin { get; set; }
    public int? TargetRepsMax { get; set; }
    public bool IsAMRAP { get; set; }
    public decimal? TargetRPE { get; set; }
    public int? RestSeconds { get; set; }
}
