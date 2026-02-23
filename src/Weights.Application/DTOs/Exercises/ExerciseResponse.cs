namespace Weights.Application.DTOs.Exercises;

public class ExerciseResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TrackType { get; set; } = string.Empty;
    public string Laterality { get; set; } = string.Empty;
    public List<ExerciseMuscleDto> Muscles { get; set; } = [];
}

public class ExerciseMuscleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TargetType { get; set; } = string.Empty;
}
