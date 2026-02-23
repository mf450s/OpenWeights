using Microsoft.EntityFrameworkCore;
using Weights.Domain.Entities;
using Weights.Infrastructure.Data;
using Weights.Infrastructure.Repositories;
using FluentAssertions;

namespace Weights.Infrastructure.Tests;
public class ExerciseRepositoryTests
{
    private readonly ApplicationDbContext _context;
    private readonly ExerciseRepository _repository;

    public ExerciseRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new ExerciseRepository(_context);
    }

    [Fact]
    public async Task GetByMuscleIdAsync_ShouldReturnExercises_ForGivenMuscle()
    {        
        // Arrange
        var targetMuscleId = 99;
        var exercise = new Exercise
        {
            Id = 1,
            Name = "Bankdrücken",
            ExerciseMuscles =
            [
                new ExerciseMuscle { MuscleId = targetMuscleId }
            ]
        };

        _context.Exercises.Add(exercise);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByMuscleIdAsync(targetMuscleId);

        // Assert
        result.Should().ContainSingle();
        result.First().Name.Should().Be("Bankdrücken");
    }

    [Fact]
    public async Task GetWithMusclesAsync_ShouldIncludeRelations()
    {
        // Arrange
        var muscle = new Muscle { Id = 5, Name = "Brust" };
        var exercise = new Exercise
        {
            Id = 10,
            Name = "Liegestütze",
            ExerciseMuscles =
            [
                new ExerciseMuscle { Muscle = muscle, MuscleId = muscle.Id }
            ]
        };

        _context.Exercises.Add(exercise);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetWithMusclesAsync(10);

        // Assert
        result.Should().NotBeNull();
        result!.ExerciseMuscles.Should().NotBeEmpty();
        result.ExerciseMuscles.First().Muscle.Name.Should().Be("Brust");
    }

    [Fact]
    public async Task GetWithMusclesAsync_ShouldReturnNull_WhenExerciseNotFound()
    {
        // Act
        var result = await _repository.GetWithMusclesAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByMuscleIdAsync_ShouldReturnEmpty_WhenNoExercisesForMuscle()
    {
        // Arrange
        var targetMuscleId = 99;
        var exercise = new Exercise
        {
            Id = 1,
            Name = "Bankdrücken",
            ExerciseMuscles =
            [
                new ExerciseMuscle { MuscleId = 1 }
            ]
        };

        _context.Exercises.Add(exercise);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByMuscleIdAsync(targetMuscleId);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllWithMusclesAsync_ShouldReturnAllExercisesWithMuscles()
    {
        // Arrange
        var muscle1 = new Muscle { Id = 1, Name = "Brust" };
        var muscle2 = new Muscle { Id = 2, Name = "Rücken" };

        var exercise1 = new Exercise
        {
            Id = 1,
            Name = "Bankdrücken",
            ExerciseMuscles =
            [
                new ExerciseMuscle { Muscle = muscle1, MuscleId = muscle1.Id }
            ]
        };

        var exercise2 = new Exercise
        {
            Id = 2,
            Name = "Rudern",
            ExerciseMuscles =
            [
                new ExerciseMuscle { Muscle = muscle2, MuscleId = muscle2.Id }
            ]
        };

        _context.Exercises.AddRange(exercise1, exercise2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllWithMusclesAsync();

        // Assert
        result.Should().HaveCount(2);
        result.ToList().ForEach(e => e.ExerciseMuscles.Should().NotBeEmpty());
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnExercise_WhenExists()
    {
        // Arrange
        var exercise = new Exercise
        {
            Id = 1,
            Name = "Kniebeugen",
            ExerciseMuscles = []
        };

        _context.Exercises.Add(exercise);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Kniebeugen");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllExercises()
    {
        // Arrange
        var exercises = new List<Exercise>
        {
            new Exercise { Id = 1, Name = "Bankdrücken", ExerciseMuscles = [] },
            new Exercise { Id = 2, Name = "Kniebeugen", ExerciseMuscles = [] },
            new Exercise { Id = 3, Name = "Kreuzheben", ExerciseMuscles = [] }
        };

        _context.Exercises.AddRange(exercises);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Select(e => e.Name).Should().Contain(new[] { "Bankdrücken", "Kniebeugen", "Kreuzheben" });
    }

    [Fact]
    public async Task AddAsync_ShouldAddExercise()
    {
        // Arrange
        var exercise = new Exercise
        {
            Id = 1,
            Name = "Schleudermaschine",
            ExerciseMuscles = []
        };

        // Act
        await _repository.AddAsync(exercise);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _context.Exercises.FindAsync(1);
        result.Should().NotBeNull();
        result!.Name.Should().Be("Schleudermaschine");
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateExercise()
    {
        // Arrange
        var exercise = new Exercise
        {
            Id = 1,
            Name = "Bankdrücken",
            ExerciseMuscles = []
        };
        _context.Exercises.Add(exercise);
        await _context.SaveChangesAsync();

        // Act
        exercise.Name = "Fliegende";
        await _repository.UpdateAsync(exercise);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _repository.GetByIdAsync(1);
        result!.Name.Should().Be("Fliegende");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveExercise()
    {
        // Arrange
        var exercise = new Exercise
        {
            Id = 1,
            Name = "Bankdrücken",
            ExerciseMuscles = []
        };
        _context.Exercises.Add(exercise);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(exercise);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _repository.GetByIdAsync(1);
        result.Should().BeNull();
    }

    [Fact]
    public async Task FindAsync_ShouldReturnMatchingExercises()
    {
        // Arrange
        var exercises = new List<Exercise>
        {
            new Exercise { Id = 1, Name = "Bankdrücken", ExerciseMuscles = [] },
            new Exercise { Id = 2, Name = "Kniebeugen", ExerciseMuscles = [] },
            new Exercise { Id = 3, Name = "Kreuzheben", ExerciseMuscles = [] }
        };

        _context.Exercises.AddRange(exercises);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.FindAsync(e => e.Name.Contains("drücken"));

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Bankdrücken");
    }

    [Fact]
    public async Task GetByMuscleIdAsync_ShouldReturnMultipleExercises_ForSameMuscle()
    {
        // Arrange
        var targetMuscleId = 1;
        var exercise1 = new Exercise
        {
            Id = 1,
            Name = "Bankdrücken",
            ExerciseMuscles = [new ExerciseMuscle { MuscleId = targetMuscleId }]
        };
        var exercise2 = new Exercise
        {
            Id = 2,
            Name = "Fliegende",
            ExerciseMuscles = [new ExerciseMuscle { MuscleId = targetMuscleId }]
        };

        _context.Exercises.AddRange(exercise1, exercise2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByMuscleIdAsync(targetMuscleId);

        // Assert
        result.Should().HaveCount(2);
        result.Select(e => e.Name).Should().Contain(["Bankdrücken", "Fliegende"]);
    }
}
