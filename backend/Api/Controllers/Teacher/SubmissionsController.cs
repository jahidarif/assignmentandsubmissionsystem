using System.IdentityModel.Tokens.Jwt;
using AssignmentSubmissionSystem.Application.Common.Interfaces;
using AssignmentSubmissionSystem.Application.Features.Teacher.Submissions.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Teacher;

[Authorize(Roles = "Teacher")]
[ApiController]
[Route("api/teacher")]
public class SubmissionsController : ControllerBase
{
    private readonly ISubmissionGradingService _submissionGradingService;

    public SubmissionsController(ISubmissionGradingService submissionGradingService)
    {
        _submissionGradingService = submissionGradingService;
    }

    private Guid CurrentTeacherId => Guid.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);

    [HttpGet("assignments/{assignmentId:guid}/submissions")]
    public async Task<IActionResult> GetSubmissionsForAssignment(Guid assignmentId, [FromQuery] int page = 1, CancellationToken cancellationToken = default)
    {
        var result = await _submissionGradingService.GetSubmissionsForAssignmentAsync(CurrentTeacherId, assignmentId, page, cancellationToken);
        return Ok(result);
    }

    [HttpGet("submissions/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _submissionGradingService.GetByIdAsync(CurrentTeacherId, id, cancellationToken);
        return Ok(result);
    }

    [HttpPatch("submissions/{id:guid}/grade")]
    public async Task<IActionResult> Grade(Guid id, [FromBody] GradeSubmissionRequest request, CancellationToken cancellationToken)
    {
        var result = await _submissionGradingService.GradeAsync(CurrentTeacherId, id, request, cancellationToken);
        return Ok(result);
    }

    [HttpPatch("submissions/{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateSubmissionStatusRequest request, CancellationToken cancellationToken)
    {
        var result = await _submissionGradingService.UpdateStatusAsync(CurrentTeacherId, id, request, cancellationToken);
        return Ok(result);
    }
}