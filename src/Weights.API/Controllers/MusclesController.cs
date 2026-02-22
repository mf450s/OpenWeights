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
}
