using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Weights.Application.DTOs.Muscles;
using Weights.Application.Interfaces;

namespace Weights.API.Controllers;

[ApiController]
[Route("api/muscles")]
[Authorize]
public class MusclesController(IMuscleService muscleService) : ControllerBase
{
    private readonly IMuscleService _muscleService = muscleService;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MuscleResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MuscleResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var muscles = await _muscleService.GetAllAsync(cancellationToken);
        return Ok(muscles);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MuscleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MuscleResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var muscle = await _muscleService.GetByIdAsync(id, cancellationToken);
        if (muscle == null)
            return Problem(detail: $"Muscle {id} not found.", statusCode: StatusCodes.Status404NotFound, title: "Not Found");
        return Ok(muscle);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _muscleService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPost]
    [ProducesResponseType(typeof(MuscleResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<MuscleResponse>> Create(MuscleCreateRequest request, CancellationToken cancellationToken)
    {
        var muscle = await _muscleService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = muscle.Id }, muscle);
    }

    [HttpPatch("{id}")]
    [ProducesResponseType(typeof(MuscleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MuscleResponse>> Update(int id, MuscleUpdateRequest request, CancellationToken cancellationToken)
    {
        var muscle = await _muscleService.UpdateAsync(id, request, cancellationToken);
        if (muscle == null)
            return Problem(detail: $"Muscle {id} not found.", statusCode: StatusCodes.Status404NotFound, title: "Not Found");
        return Ok(muscle);
    }
}
