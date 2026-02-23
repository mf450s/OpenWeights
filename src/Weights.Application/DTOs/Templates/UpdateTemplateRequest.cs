using System.ComponentModel.DataAnnotations;

namespace Weights.Application.DTOs.Templates;

public class UpdateTemplateRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    public List<TemplateExerciseDto> Exercises { get; set; } = [];
}
