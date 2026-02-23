using System.ComponentModel.DataAnnotations;

namespace Weights.Application.DTOs.Muscles;

public class MuscleCreateRequest
{
    [Required(ErrorMessage = "Muscle name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Muscle name must be between 2 and 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public string? BodyPart { get; set; }
}