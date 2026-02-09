using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Weights.Application.DTOs.Exercises;
using Weights.Application.Interfaces;

namespace Weights.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExercisesController(IExerciseService exerciseService) : ControllerBase
{
    private readonly IExerciseService _exerciseService = exerciseService;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ExerciseResponse>>> GetAll([FromQuery] int? muscleId, CancellationToken cancellationToken)
    {
        var exercises = await _exerciseService.GetAllAsync(muscleId, cancellationToken);
        return Ok(exercises);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ExerciseResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var exercise = await _exerciseService.GetByIdAsync(id, cancellationToken);
        
        if (exercise == null)
            return NotFound();

        return Ok(exercise);
    }
}
