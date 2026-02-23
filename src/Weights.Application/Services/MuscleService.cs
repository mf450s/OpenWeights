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

    public async Task<MuscleResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var muscle = await _unitOfWork.Muscles.GetByIdAsync(id, cancellationToken);
        if (muscle == null)
            return null;

        return new MuscleResponse
        {
            Id = muscle.Id,
            Name = muscle.Name,
            BodyPart = muscle.BodyPart
        };
    }

    public async Task<MuscleResponse> CreateAsync(MuscleCreateRequest request, CancellationToken cancellationToken = default)
    {
        var muscle = new Domain.Entities.Muscle
        {
            Name = request.Name,
            BodyPart = request.BodyPart ?? string.Empty
        };

        await _unitOfWork.Muscles.AddAsync(muscle, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new MuscleResponse
        {
            Id = muscle.Id,
            Name = muscle.Name,
            BodyPart = muscle.BodyPart
        };
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var muscle = await _unitOfWork.Muscles.GetByIdAsync(id, cancellationToken);
        if (muscle == null)
            return;
        await _unitOfWork.Muscles.DeleteAsync(muscle, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<MuscleResponse?> UpdateAsync(int id, MuscleUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var muscle = await _unitOfWork.Muscles.GetByIdAsync(id, cancellationToken);
        if (muscle == null)
            return null;

        muscle.Name = request.Name;
        muscle.BodyPart = request.BodyPart ?? string.Empty;

        await _unitOfWork.Muscles.UpdateAsync(muscle, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new MuscleResponse
        {
            Id = muscle.Id,
            Name = muscle.Name,
            BodyPart = muscle.BodyPart
        };
    }
}
