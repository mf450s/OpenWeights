using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Weights.Application.DTOs.Common;
using Weights.Application.DTOs.Exercises;
using Weights.Application.Interfaces;

namespace Weights.API.Controllers;

[ApiController]
[Route("api/exercises")]
[Authorize]
public class ExercisesController(IExerciseService exerciseService) : ControllerBase
{
    private readonly IExerciseService _exerciseService = exerciseService;

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<ExerciseResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<ExerciseResponse>>> GetAll(
        [FromQuery] int? muscleId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _exerciseService.GetAllAsync(muscleId, page, pageSize, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ExerciseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ExerciseResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var exercise = await _exerciseService.GetByIdAsync(id, cancellationToken);

        if (exercise == null)
            return Problem(detail: $"Exercise {id} not found.", statusCode: StatusCodes.Status404NotFound, title: "Not Found");

        return Ok(exercise);
    }

    [HttpGet("{id}/history")]
    [ProducesResponseType(typeof(ExerciseHistoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ExerciseHistoryResponse>> GetHistory(
        int id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var userId = GetUserId();
        var result = await _exerciseService.GetHistoryAsync(id, userId, page, pageSize, cancellationToken);

        if (result == null)
            return Problem(detail: $"Exercise {id} not found.", statusCode: StatusCodes.Status404NotFound, title: "Not Found");

        return Ok(result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var result = await _exerciseService.DeleteAsync(id, userId, cancellationToken);

        if (!result)
            return Problem(detail: $"Exercise {id} not found.", statusCode: StatusCodes.Status404NotFound, title: "Not Found");

        return NoContent();
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(ExerciseCreateRequest request, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        await _exerciseService.CreateAsync(request, userId, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = 1 }, request);
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim!);
    }
}
