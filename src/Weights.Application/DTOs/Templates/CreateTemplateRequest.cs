using System.ComponentModel.DataAnnotations;

namespace Weights.Application.DTOs.Templates;

public class CreateTemplateRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    public List<TemplateExerciseDto> Exercises { get; set; } = [];
}

public class TemplateExerciseDto
{
    [Required]
    public int ExerciseId { get; set; }

    [Required]
    public int OrderIndex { get; set; }

    [Required]
    [Range(1, 100)]
    public int TargetSets { get; set; }

    [Range(1, 9999)]
    public int? TargetRepsMin { get; set; }

    [Range(1, 9999)]
    public int? TargetRepsMax { get; set; }

    public bool IsAMRAP { get; set; } = false;

    [Range(0, 10)]
    public decimal? TargetRPE { get; set; }

    [Range(0, 600)]
    public int? RestSeconds { get; set; }
}
