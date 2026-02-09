namespace Weights.Application.DTOs.Templates;

public class TemplateResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<TemplateExerciseResponse> Exercises { get; set; } = new();
}

public class TemplateExerciseResponse
{
    public int ExerciseId { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
    public int TargetSets { get; set; }
    public string TargetReps { get; set; } = string.Empty;
    public decimal? TargetRPE { get; set; }
    public int? RestSeconds { get; set; }
}
