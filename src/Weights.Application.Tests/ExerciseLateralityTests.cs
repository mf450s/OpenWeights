using Weights.Domain.Entities;
using Weights.Domain.Enums;
using Xunit;

namespace Weights.Application.Tests;

public class ExerciseLateralityTests
{
    [Fact]
    public void Exercise_DefaultLaterality_ShouldBeBilateral()
    {
        // Arrange & Act
        var exercise = new Exercise
        {
            Name = "Bench Press",
            TrackType = TrackType.WeightReps
        };

        // Assert
        Assert.Equal(Laterality.Bilateral, exercise.Laterality);
    }

    [Fact]
    public void Exercise_CanSetLaterality_ToUnilateral()
    {
        // Arrange & Act
        var exercise = new Exercise
        {
            Name = "Dumbbell Curl",
            TrackType = TrackType.WeightReps,
            Laterality = Laterality.Unilateral
        };

        // Assert
        Assert.Equal(Laterality.Unilateral, exercise.Laterality);
    }

    [Fact]
    public void SetHistory_Side_CanBeNull()
    {
        // Arrange & Act
        var setHistory = new SetHistory
        {
            ExerciseId = 1,
            WorkoutSessionId = 1,
            SetNumber = 1,
            Weight = 60,
            Reps = 10,
            Side = null,
            PerformedAt = DateTime.UtcNow
        };

        // Assert
        Assert.Null(setHistory.Side);
    }

    [Fact]
    public void SetHistory_Side_CanBeSet_ToLeft()
    {
        // Arrange & Act
        var setHistory = new SetHistory
        {
            ExerciseId = 1,
            WorkoutSessionId = 1,
            SetNumber = 1,
            Weight = 10,
            Reps = 10,
            Side = Side.Left,
            PerformedAt = DateTime.UtcNow
        };

        // Assert
        Assert.Equal(Side.Left, setHistory.Side);
    }

    [Fact]
    public void SetHistory_Side_CanBeSet_ToRight()
    {
        // Arrange & Act
        var setHistory = new SetHistory
        {
            ExerciseId = 1,
            WorkoutSessionId = 1,
            SetNumber = 1,
            Weight = 10,
            Reps = 10,
            Side = Side.Right,
            PerformedAt = DateTime.UtcNow
        };

        // Assert
        Assert.Equal(Side.Right, setHistory.Side);
    }
}
