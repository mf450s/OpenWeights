using Microsoft.EntityFrameworkCore.Storage;
using Weights.Domain.Interfaces;
using Weights.Infrastructure.Data;

namespace Weights.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    public IUserRepository Users { get; }
    public IMuscleRepository Muscles { get; }
    public IExerciseRepository Exercises { get; }
    public IWorkoutTemplateRepository WorkoutTemplates { get; }
    public IWorkoutSessionRepository WorkoutSessions { get; }

    public UnitOfWork(
        ApplicationDbContext context,
        IUserRepository users,
        IMuscleRepository muscles,
        IExerciseRepository exercises,
        IWorkoutTemplateRepository workoutTemplates,
        IWorkoutSessionRepository workoutSessions)
    {
        _context = context;
        Users = users;
        Muscles = muscles;
        Exercises = exercises;
        WorkoutTemplates = workoutTemplates;
        WorkoutSessions = workoutSessions;
    }

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

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
