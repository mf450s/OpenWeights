using Microsoft.EntityFrameworkCore.Storage;
using Weights.Domain.Interfaces;
using Weights.Infrastructure.Data;

namespace Weights.Infrastructure.Repositories;

public class UnitOfWork(
    ApplicationDbContext context,
    IUserRepository users,
    IMuscleRepository muscles,
    IExerciseRepository exercises,
    IWorkoutTemplateRepository workoutTemplates,
    IWorkoutSessionRepository workoutSessions) : IUnitOfWork
{
    private readonly ApplicationDbContext _context = context;
    private IDbContextTransaction? _transaction;

    public IUserRepository Users { get; } = users;
    public IMuscleRepository Muscles { get; } = muscles;
    public IExerciseRepository Exercises { get; } = exercises;
    public IWorkoutTemplateRepository WorkoutTemplates { get; } = workoutTemplates;
    public IWorkoutSessionRepository WorkoutSessions { get; } = workoutSessions;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task DeleteAsync(CancellationToken cancellationToken = default)
    {
        await _context.Database.EnsureDeletedAsync(cancellationToken);
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
