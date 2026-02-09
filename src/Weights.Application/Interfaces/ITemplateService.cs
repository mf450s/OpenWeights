using Weights.Application.DTOs.Templates;

namespace Weights.Application.Interfaces;

public interface ITemplateService
{
    Task<IEnumerable<WorkoutTemplateListResponse>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<TemplateResponse> CreateAsync(Guid userId, CreateTemplateRequest request, CancellationToken cancellationToken = default);
    Task<TemplateResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
