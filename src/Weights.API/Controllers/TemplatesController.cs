using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Weights.Application.DTOs.Common;
using Weights.Application.DTOs.Templates;
using Weights.Application.Interfaces;

namespace Weights.API.Controllers;

[ApiController]
[Route("api/templates")]
[Authorize]
public class TemplatesController(ITemplateService templateService) : ControllerBase
{
    private readonly ITemplateService _templateService = templateService;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<WorkoutTemplateListResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<WorkoutTemplateListResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var templates = await _templateService.GetByUserIdAsync(userId, cancellationToken);
        return Ok(templates);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TemplateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TemplateResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var template = await _templateService.GetByIdAsync(id, cancellationToken);

        if (template == null)
            return Problem(detail: $"Template {id} not found.", statusCode: StatusCodes.Status404NotFound, title: "Not Found");

        return Ok(template);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TemplateResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TemplateResponse>> Create([FromBody] CreateTemplateRequest request, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var result = await _templateService.CreateAsync(userId, request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(TemplateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TemplateResponse>> Update(int id, [FromBody] UpdateTemplateRequest request, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var result = await _templateService.UpdateAsync(userId, id, request, cancellationToken);

        if (result == null)
            return Problem(detail: $"Template {id} not found or not owned by user.", statusCode: StatusCodes.Status404NotFound, title: "Not Found");

        return Ok(result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Archive(int id, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var success = await _templateService.ArchiveAsync(userId, id, cancellationToken);

        if (!success)
            return Problem(detail: $"Template {id} not found or not owned by user.", statusCode: StatusCodes.Status404NotFound, title: "Not Found");

        return NoContent();
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim!);
    }
}
