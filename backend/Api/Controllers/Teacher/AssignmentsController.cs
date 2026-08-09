using System.IdentityModel.Tokens.Jwt;
using AssignmentSubmissionSystem.Application.Common.Interfaces;
using AssignmentSubmissionSystem.Application.Features.Teacher.Assignments.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Teacher;

[Authorize(Roles = "Teacher")]
[ApiController]
[Route("api/teacher/assignments")]
public class AssignmentsController : ControllerBase
{
    private readonly IAssignmentManagementService _assignmentManagementService;

    public AssignmentsController(IAssignmentManagementService assignmentManagementService)
    {
        _assignmentManagementService = assignmentManagementService;
    }

    private Guid CurrentTeacherId => Guid.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);

    [HttpGet]
    public async Task<IActionResult> GetAssignments([FromQuery] int page = 1, CancellationToken cancellationToken = default)
    {
        var result = await _assignmentManagementService.GetAssignmentsAsync(CurrentTeacherId, page, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _assignmentManagementService.GetByIdAsync(CurrentTeacherId, id, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAssignmentRequest request, CancellationToken cancellationToken)
    {
        var result = await _assignmentManagementService.CreateAsync(CurrentTeacherId, request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAssignmentRequest request, CancellationToken cancellationToken)
    {
        var result = await _assignmentManagementService.UpdateAsync(CurrentTeacherId, id, request, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _assignmentManagementService.DeleteAsync(CurrentTeacherId, id, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id:guid}/publish")]
    public async Task<IActionResult> Publish(Guid id, CancellationToken cancellationToken)
    {
        var result = await _assignmentManagementService.PublishAsync(CurrentTeacherId, id, cancellationToken);
        return Ok(result);
    }
}