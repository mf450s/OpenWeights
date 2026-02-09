using Microsoft.EntityFrameworkCore;
using Weights.Domain.Entities;
using Weights.Domain.Interfaces;
using Weights.Infrastructure.Data;

namespace Weights.Infrastructure.Repositories;

public class MuscleRepository : Repository<Muscle, int>, IMuscleRepository
{
    public MuscleRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Muscle>> GetByBodyPartAsync(string bodyPart, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(m => m.BodyPart == bodyPart)
            .ToListAsync(cancellationToken);
    }
}
