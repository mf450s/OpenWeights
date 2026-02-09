using Microsoft.EntityFrameworkCore;
using Weights.Domain.Entities;
using Weights.Domain.Interfaces;
using Weights.Infrastructure.Data;

namespace Weights.Infrastructure.Repositories;

public class ExerciseRepository : Repository<Exercise, int>, IExerciseRepository
{
    public ExerciseRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Exercise>> GetByMuscleIdAsync(int muscleId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(e => e.ExerciseMuscles)
                .ThenInclude(em => em.Muscle)
            .Where(e => e.ExerciseMuscles.Any(em => em.MuscleId == muscleId))
            .ToListAsync(cancellationToken);
    }

    public async Task<Exercise?> GetWithMusclesAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(e => e.ExerciseMuscles)
                .ThenInclude(em => em.Muscle)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Exercise>> GetAllWithMusclesAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(e => e.ExerciseMuscles)
                .ThenInclude(em => em.Muscle)
            .ToListAsync(cancellationToken);
    }
}
