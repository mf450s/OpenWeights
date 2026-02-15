using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Weights.Domain.Entities;
using Weights.Infrastructure.Data;
using Weights.Infrastructure.Repositories;

namespace Weights.Infrastructure.Tests;

public class RepositoryTests
{
    private readonly ApplicationDbContext _context;
    private readonly Repository<Muscle, int> _repository;

    public RepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new Repository<Muscle, int>(_context);
    }

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEntity_WhenEntityExists()
    {
        // Arrange
        var muscle = new Muscle { Id = 1, Name = "Biceps", BodyPart = "Arms" };
        _context.Muscles.Add(muscle);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Biceps");
        result.BodyPart.Should().Be("Arms");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenEntityDoesNotExist()
    {
        // Arrange
        var muscle = new Muscle { Id = 1, Name = "Biceps", BodyPart = "Arms" };
        _context.Muscles.Add(muscle);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNoEntitiesExist()
    {
        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WithDifferentIds_ShouldReturnCorrectEntity()
    {
        // Arrange
        var muscle1 = new Muscle { Id = 1, Name = "Biceps", BodyPart = "Arms" };
        var muscle2 = new Muscle { Id = 2, Name = "Triceps", BodyPart = "Arms" };
        _context.Muscles.AddRange(muscle1, muscle2);
        await _context.SaveChangesAsync();

        // Act
        var result1 = await _repository.GetByIdAsync(1);
        var result2 = await _repository.GetByIdAsync(2);

        // Assert
        result1.Should().NotBeNull();
        result1!.Name.Should().Be("Biceps");
        result2.Should().NotBeNull();
        result2!.Name.Should().Be("Triceps");
    }

    #endregion

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEntities_WhenEntitiesExist()
    {
        // Arrange
        var muscle1 = new Muscle { Id = 1, Name = "Biceps", BodyPart = "Arms" };
        var muscle2 = new Muscle { Id = 2, Name = "Triceps", BodyPart = "Arms" };
        var muscle3 = new Muscle { Id = 3, Name = "Quadriceps", BodyPart = "Legs" };

        _context.Muscles.AddRange(muscle1, muscle2, muscle3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(m => m.Id == 1 && m.Name == "Biceps");
        result.Should().Contain(m => m.Id == 2 && m.Name == "Triceps");
        result.Should().Contain(m => m.Id == 3 && m.Name == "Quadriceps");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoEntitiesExist()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnOnlyTheRequestedEntityType()
    {
        // Arrange
        var muscle1 = new Muscle { Id = 1, Name = "Biceps", BodyPart = "Arms" };
        var muscle2 = new Muscle { Id = 2, Name = "Triceps", BodyPart = "Arms" };

        _context.Muscles.AddRange(muscle1, muscle2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEntitiesInInsertionOrder()
    {
        // Arrange
        var muscle1 = new Muscle { Id = 1, Name = "Biceps", BodyPart = "Arms" };
        var muscle2 = new Muscle { Id = 2, Name = "Triceps", BodyPart = "Arms" };
        var muscle3 = new Muscle { Id = 3, Name = "Quadriceps", BodyPart = "Legs" };

        _context.Muscles.AddRange(muscle1, muscle2, muscle3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();
        var list = result.ToList();

        // Assert
        list.Should().HaveCount(3);
        list[0].Id.Should().Be(1);
        list[1].Id.Should().Be(2);
        list[2].Id.Should().Be(3);
    }

    #endregion

    #region FindAsync Tests

    [Fact]
    public async Task FindAsync_ShouldReturnMatchingEntities_WhenPredicateMatches()
    {
        // Arrange
        var muscle1 = new Muscle { Id = 1, Name = "Biceps", BodyPart = "Arms" };
        var muscle2 = new Muscle { Id = 2, Name = "Triceps", BodyPart = "Arms" };
        var muscle3 = new Muscle { Id = 3, Name = "Quadriceps", BodyPart = "Legs" };

        _context.Muscles.AddRange(muscle1, muscle2, muscle3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.FindAsync(m => m.BodyPart == "Arms");

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(m => m.Name == "Biceps");
        result.Should().Contain(m => m.Name == "Triceps");
    }

    [Fact]
    public async Task FindAsync_ShouldReturnEmptyList_WhenNoEntitiesMatch()
    {
        // Arrange
        var muscle = new Muscle { Id = 1, Name = "Biceps", BodyPart = "Arms" };
        _context.Muscles.Add(muscle);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.FindAsync(m => m.BodyPart == "Legs");

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task FindAsync_ShouldReturnEmptyList_WhenNoEntitiesExist()
    {
        // Act
        var result = await _repository.FindAsync(m => m.BodyPart == "Arms");

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task FindAsync_ShouldWorkWithComplexPredicates()
    {
        // Arrange
        var muscle1 = new Muscle { Id = 1, Name = "Biceps", BodyPart = "Arms" };
        var muscle2 = new Muscle { Id = 2, Name = "Triceps", BodyPart = "Arms" };
        var muscle3 = new Muscle { Id = 3, Name = "Quadriceps", BodyPart = "Legs" };

        _context.Muscles.AddRange(muscle1, muscle2, muscle3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.FindAsync(m => m.BodyPart == "Arms" && m.Name.Contains("B"));

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(m => m.Name == "Biceps");
    }

    [Fact]
    public async Task FindAsync_ShouldReturnEntitiesMatchingIdPredicate()
    {
        // Arrange
        var muscle1 = new Muscle { Id = 1, Name = "Biceps", BodyPart = "Arms" };
        var muscle2 = new Muscle { Id = 2, Name = "Triceps", BodyPart = "Arms" };
        var muscle3 = new Muscle { Id = 3, Name = "Quadriceps", BodyPart = "Legs" };

        _context.Muscles.AddRange(muscle1, muscle2, muscle3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.FindAsync(m => m.Id > 1);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(m => m.Id == 2);
        result.Should().Contain(m => m.Id == 3);
    }

    #endregion

    #region AddAsync Tests

    [Fact]
    public async Task AddAsync_ShouldAddEntity_AndReturnIt()
    {
        // Arrange
        var muscle = new Muscle { Id = 1, Name = "Biceps", BodyPart = "Arms" };

        // Act
        var result = await _repository.AddAsync(muscle);

        // Assert
        result.Should().Be(muscle);
        result.Id.Should().Be(1);
        result.Name.Should().Be("Biceps");
    }

    [Fact]
    public async Task AddAsync_ShouldStageEntityForInsertion_NotPersistImmediately()
    {
        // Arrange
        var muscle = new Muscle { Id = 1, Name = "Biceps", BodyPart = "Arms" };

        // Act
        await _repository.AddAsync(muscle);

        // Assert - without SaveChanges, entity should not be in database
        // Verify by using a new repository instance with fresh query
        var repository2 = new Repository<Muscle, int>(_context);
        var found = await repository2.GetByIdAsync(1);
        
        // Before SaveChanges, the entity is only in memory
        // The repository method is synchronous for in-memory, so it should be available in same context
        // But not saved to database yet
        _context.ChangeTracker.Clear();
        found = await repository2.GetByIdAsync(1);
        found.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_ShouldAllowAddingMultipleEntities()
    {
        // Arrange
        var muscle1 = new Muscle { Id = 1, Name = "Biceps", BodyPart = "Arms" };
        var muscle2 = new Muscle { Id = 2, Name = "Triceps", BodyPart = "Arms" };

        // Act
        var result1 = await _repository.AddAsync(muscle1);
        var result2 = await _repository.AddAsync(muscle2);
        await _context.SaveChangesAsync();

        // Assert
        result1.Should().Be(muscle1);
        result2.Should().Be(muscle2);

        var all = await _repository.GetAllAsync();
        all.Should().HaveCount(2);
    }

    [Fact]
    public async Task AddAsync_ShouldSetNewEntityState()
    {
        // Arrange
        var muscle = new Muscle { Id = 1, Name = "Biceps", BodyPart = "Arms" };

        // Act
        await _repository.AddAsync(muscle);

        // Assert - entity should be in Added state
        var entry = _context.Entry(muscle);
        entry.State.Should().Be(Microsoft.EntityFrameworkCore.EntityState.Added);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ShouldMarkEntityForUpdate()
    {
        // Arrange
        var muscle = new Muscle { Id = 1, Name = "Biceps", BodyPart = "Arms" };
        _context.Muscles.Add(muscle);
        await _context.SaveChangesAsync();

        // Act
        muscle.Name = "Biceps Updated";
        await _repository.UpdateAsync(muscle);
        await _context.SaveChangesAsync();

        // Assert
        var updated = await _repository.GetByIdAsync(1);
        updated.Should().NotBeNull();
        updated!.Name.Should().Be("Biceps Updated");
    }

    [Fact]
    public async Task UpdateAsync_ShouldNotPersistImmediately()
    {
        // Arrange
        var muscle = new Muscle { Id = 1, Name = "Biceps", BodyPart = "Arms" };
        _context.Muscles.Add(muscle);
        await _context.SaveChangesAsync();

        // Act
        muscle.Name = "Biceps Updated";
        await _repository.UpdateAsync(muscle);

        // Assert - without SaveChanges, change should not be persisted
        var entry = _context.Entry(muscle);
        entry.State.Should().Be(Microsoft.EntityFrameworkCore.EntityState.Modified);
        
        // Verify the original value still exists in the database
        _context.ChangeTracker.Clear();
        var fromDb = await _repository.GetByIdAsync(1);
        fromDb!.Name.Should().Be("Biceps"); // Original value still in database
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateMultipleProperties()
    {
        // Arrange
        var muscle = new Muscle { Id = 1, Name = "Biceps", BodyPart = "Arms" };
        _context.Muscles.Add(muscle);
        await _context.SaveChangesAsync();

        // Act
        muscle.Name = "Biceps Updated";
        muscle.BodyPart = "Upper Arms";
        await _repository.UpdateAsync(muscle);
        await _context.SaveChangesAsync();

        // Assert
        var updated = await _repository.GetByIdAsync(1);
        updated.Should().NotBeNull();
        updated!.Name.Should().Be("Biceps Updated");
        updated!.BodyPart.Should().Be("Upper Arms");
    }

    [Fact]
    public async Task UpdateAsync_ShouldSetModifiedState()
    {
        // Arrange
        var muscle = new Muscle { Id = 1, Name = "Biceps", BodyPart = "Arms" };
        _context.Muscles.Add(muscle);
        await _context.SaveChangesAsync();

        // Clear change tracker to reset state
        _context.ChangeTracker.Clear();
        muscle = (await _repository.GetByIdAsync(1))!;

        // Act
        muscle.Name = "Updated";
        await _repository.UpdateAsync(muscle);

        // Assert
        var entry = _context.Entry(muscle);
        entry.State.Should().Be(Microsoft.EntityFrameworkCore.EntityState.Modified);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_ShouldMarkEntityForDeletion()
    {
        // Arrange
        var muscle = new Muscle { Id = 1, Name = "Biceps", BodyPart = "Arms" };
        _context.Muscles.Add(muscle);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(muscle);
        await _context.SaveChangesAsync();

        // Assert
        var deleted = await _repository.GetByIdAsync(1);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldNotPersistImmediately()
    {
        // Arrange
        var muscle = new Muscle { Id = 1, Name = "Biceps", BodyPart = "Arms" };
        _context.Muscles.Add(muscle);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(muscle);

        // Assert - entity should still be found in current context before SaveChanges
        var beforeSave = await _repository.GetByIdAsync(1);
        beforeSave.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteMultipleEntities()
    {
        // Arrange
        var muscle1 = new Muscle { Id = 1, Name = "Biceps", BodyPart = "Arms" };
        var muscle2 = new Muscle { Id = 2, Name = "Triceps", BodyPart = "Arms" };
        _context.Muscles.AddRange(muscle1, muscle2);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(muscle1);
        await _repository.DeleteAsync(muscle2);
        await _context.SaveChangesAsync();

        // Assert
        var all = await _repository.GetAllAsync();
        all.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteAsync_ShouldSetDeletedState()
    {
        // Arrange
        var muscle = new Muscle { Id = 1, Name = "Biceps", BodyPart = "Arms" };
        _context.Muscles.Add(muscle);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(muscle);

        // Assert
        var entry = _context.Entry(muscle);
        entry.State.Should().Be(Microsoft.EntityFrameworkCore.EntityState.Deleted);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public async Task CRUD_Operations_ShouldWorkTogether()
    {
        // Arrange
        var muscle = new Muscle { Id = 1, Name = "Biceps", BodyPart = "Arms" };

        // Act - Create
        await _repository.AddAsync(muscle);
        await _context.SaveChangesAsync();
        var created = await _repository.GetByIdAsync(1);

        // Assert Create
        created.Should().NotBeNull();
        created!.Name.Should().Be("Biceps");

        // Act - Update
        created.Name = "Biceps Double Peak";
        await _repository.UpdateAsync(created);
        await _context.SaveChangesAsync();
        var updated = await _repository.GetByIdAsync(1);

        // Assert Update
        updated.Should().NotBeNull();
        updated!.Name.Should().Be("Biceps Double Peak");

        // Act - Delete
        await _repository.DeleteAsync(updated);
        await _context.SaveChangesAsync();
        var deleted = await _repository.GetByIdAsync(1);

        // Assert Delete
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task Multiple_Operations_ShouldMaintainDataConsistency()
    {
        // Arrange
        var muscles = new[]
        {
            new Muscle { Id = 1, Name = "Biceps", BodyPart = "Arms" },
            new Muscle { Id = 2, Name = "Triceps", BodyPart = "Arms" },
            new Muscle { Id = 3, Name = "Quadriceps", BodyPart = "Legs" }
        };

        // Act
        foreach (var muscle in muscles)
        {
            await _repository.AddAsync(muscle);
        }
        await _context.SaveChangesAsync();

        var found = await _repository.FindAsync(m => m.BodyPart == "Arms");
        var first = found.First();
        first.Name = "Biceps Updated";
        await _repository.UpdateAsync(first);
        await _context.SaveChangesAsync();

        await _repository.DeleteAsync(muscles[2]);
        await _context.SaveChangesAsync();

        var remaining = await _repository.GetAllAsync();

        // Assert
        remaining.Should().HaveCount(2);
        remaining.Should().Contain(m => m.Name == "Biceps Updated");
        remaining.Should().NotContain(m => m.Id == 3);
    }

    #endregion
}
