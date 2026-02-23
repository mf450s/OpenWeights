using System.ComponentModel.DataAnnotations;
using Weights.Domain.Entities;
using Weights.Domain.Enums;

namespace Weights.Application.DTOs.Exercises;

public class ExerciseCreateRequest
{
    [Required(ErrorMessage = "Exercise name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Exercise name must be between 1 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Track type is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Track type must be between 1 and 50 characters")]
    public TrackType TrackType { get; set; }

    public Laterality Laterality { get; set; } = Laterality.Bilateral;
    [Required(ErrorMessage = "Exercise muscles are required")]
    public ICollection<ExerciseMuscle> ExerciseMuscles { get; set; } = [];
}