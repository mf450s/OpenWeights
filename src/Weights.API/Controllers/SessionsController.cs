using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Weights.Application.DTOs.Sessions;
using Weights.Application.Interfaces;

namespace Weights.API.Controllers;

[ApiController]
[Route("api/sessions")]
[Authorize]
public class SessionsController(ISessionService sessionService) : ControllerBase
{
    private readonly ISessionService _sessionService = sessionService;

    [HttpPost]
    [ProducesResponseType(typeof(SessionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SessionResponse>> Create([FromBody] CreateSessionRequest request, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var result = await _sessionService.CreateAsync(userId, request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SessionDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SessionDetailResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var result = await _sessionService.GetByIdAsync(id, userId, cancellationToken);

        if (result == null)
            return Problem(detail: $"Session {id} not found.", statusCode: StatusCodes.Status404NotFound, title: "Not Found");

        return Ok(result);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(SessionDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SessionDetailResponse>> Update(int id, [FromBody] UpdateSessionRequest request, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var result = await _sessionService.UpdateAsync(id, userId, request, cancellationToken);

        if (result == null)
            return Problem(detail: $"Session {id} not found or not owned by user.", statusCode: StatusCodes.Status404NotFound, title: "Not Found");

        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(SessionHistoryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<SessionHistoryResponse>> GetHistory(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var userId = GetUserId();
        var result = await _sessionService.GetHistoryAsync(userId, page, pageSize, cancellationToken);
        return Ok(result);
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim!);
    }
}
