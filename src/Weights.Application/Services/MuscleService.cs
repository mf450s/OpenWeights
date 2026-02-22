using Weights.Application.DTOs.Muscles;
using Weights.Application.Interfaces;
using Weights.Domain.Interfaces;

namespace Weights.Application.Services;

public class MuscleService(IUnitOfWork unitOfWork) : IMuscleService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IEnumerable<MuscleResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var muscles = await _unitOfWork.Muscles.GetAllAsync(cancellationToken);

        return muscles.Select(m => new MuscleResponse
        {
            Id = m.Id,
            Name = m.Name,
            BodyPart = m.BodyPart
        });
    }
}
