namespace Weights.Domain.Entities;

public abstract class Entity<TId>
{
    public TId Id { get; set; } = default!;
}
