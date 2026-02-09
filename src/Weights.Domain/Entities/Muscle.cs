namespace Weights.Domain.Entities;

public class Muscle : Entity<int>
{
    public string Name { get; set; } = string.Empty;
    public string? BodyPart { get; set; }

    // Navigation properties
    public ICollection<ExerciseMuscle> ExerciseMuscles { get; set; } = new List<ExerciseMuscle>();
}
