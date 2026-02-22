using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Weights.Domain.Entities;
using Weights.Infrastructure.Data;
using Weights.Infrastructure.Repositories;

namespace Weights.Infrastructure.Tests;

public class MuscleRepositoryTests
{
    private readonly ApplicationDbContext _context;
    private readonly MuscleRepository _repository;

    public MuscleRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new MuscleRepository(_context);
    }

    [Fact]
    public async Task GetByBodyPartAsync_ShouldReturnMuscles_WhenMusclesExist()
    {
        // Arrange
        var muscle1 = new Muscle { Id = 1, Name = "Biceps", BodyPart = "Arms" };
        var muscle2 = new Muscle { Id = 2, Name = "Triceps", BodyPart = "Arms" };
        var muscle3 = new Muscle { Id = 3, Name = "Quadriceps", BodyPart = "Legs" };

        _context.Muscles.AddRange(muscle1, muscle2, muscle3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByBodyPartAsync("Arms");

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(m => m.Name == "Biceps");
        result.Should().Contain(m => m.Name == "Triceps");
    }

    [Fact]
    public async Task GetByBodyPartAsync_ShouldReturnEmptyList_WhenNoMusclesExist()
    {
        // Arrange
        var muscle = new Muscle { Id = 1, Name = "Biceps", BodyPart = "Arms" };
        _context.Muscles.Add(muscle);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByBodyPartAsync("NonExistentPart");

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByBodyPartAsync_ShouldBeCaseSensitive()
    {
        // Arrange
        var muscle = new Muscle { Id = 1, Name = "Biceps", BodyPart = "Arms" };
        _context.Muscles.Add(muscle);
        await _context.SaveChangesAsync();

        // Act
        var resultLowercase = await _repository.GetByBodyPartAsync("arms");
        var resultMixedcase = await _repository.GetByBodyPartAsync("ARMS");

        // Assert
        resultLowercase.Should().BeEmpty();
        resultMixedcase.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnMuscle_WhenExists()
    {
        // Arrange
        var muscle = new Muscle { Id = 5, Name = "Biceps", BodyPart = "Arms" };
        _context.Muscles.Add(muscle);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(5);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Biceps");
        result.BodyPart.Should().Be("Arms");
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
    public async Task GetAllAsync_ShouldReturnAllMuscles()
    {
        // Arrange
        var muscles = new[]
        {
            new Muscle { Id = 1, Name = "Biceps", BodyPart = "Arms" },
            new Muscle { Id = 2, Name = "Triceps", BodyPart = "Arms" },
            new Muscle { Id = 3, Name = "Quadriceps", BodyPart = "Legs" }
        };
        _context.Muscles.AddRange(muscles);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmpty_WhenNoMusclesExist()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task AddAsync_ShouldAddMuscle()
    {
        // Arrange
        var muscle = new Muscle { Id = 1, Name = "Pecs", BodyPart = "Chest" };

        // Act
        await _repository.AddAsync(muscle);
        await _context.SaveChangesAsync();

        // Assert
        var stored = await _context.Muscles.FindAsync(1);
        stored.Should().NotBeNull();
        stored!.Name.Should().Be("Pecs");
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateMuscle()
    {
        // Arrange
        var muscle = new Muscle { Id = 1, Name = "Biceps", BodyPart = "Arms" };
        _context.Muscles.Add(muscle);
        await _context.SaveChangesAsync();

        // Act
        muscle.Name = "Updated Biceps";
        await _repository.UpdateAsync(muscle);
        await _context.SaveChangesAsync();

        // Assert
        var updated = await _context.Muscles.FindAsync(1);
        updated!.Name.Should().Be("Updated Biceps");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveMuscle()
    {
        // Arrange
        var muscle = new Muscle { Id = 1, Name = "Biceps", BodyPart = "Arms" };
        _context.Muscles.Add(muscle);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(muscle);
        await _context.SaveChangesAsync();

        // Assert
        var deleted = await _context.Muscles.FindAsync(1);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task FindAsync_ShouldReturnMatchingMuscles()
    {
        // Arrange
        var muscles = new[]
        {
            new Muscle { Id = 1, Name = "Biceps", BodyPart = "Arms" },
            new Muscle { Id = 2, Name = "Triceps", BodyPart = "Arms" },
            new Muscle { Id = 3, Name = "Quadriceps", BodyPart = "Legs" }
        };
        _context.Muscles.AddRange(muscles);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.FindAsync(m => m.BodyPart == "Arms");

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(m => m.BodyPart == "Arms");
    }

}