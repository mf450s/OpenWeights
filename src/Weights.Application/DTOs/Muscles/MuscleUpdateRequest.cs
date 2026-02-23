using System.ComponentModel.DataAnnotations;

namespace Weights.Application.DTOs.Muscles;

public class MuscleUpdateRequest
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "BodyPart must not exceed 500 characters")]
    public string? BodyPart { get; set; }
}