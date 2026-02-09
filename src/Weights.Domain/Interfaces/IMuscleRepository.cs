using Weights.Domain.Entities;

namespace Weights.Domain.Interfaces;

public interface IMuscleRepository : IRepository<Muscle, int>
{
    Task<IEnumerable<Muscle>> GetByBodyPartAsync(string bodyPart, CancellationToken cancellationToken = default);
}
