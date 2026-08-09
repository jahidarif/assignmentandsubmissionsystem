using System.IdentityModel.Tokens.Jwt;
using AssignmentSubmissionSystem.Application.Common.Interfaces;
using AssignmentSubmissionSystem.Application.Features.Student.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Student;

[Authorize(Roles = "Student")]
[ApiController]
[Route("api/student")]
public class SubmissionsController : ControllerBase
{
    private readonly IStudentSubmissionService _studentSubmissionService;

    public SubmissionsController(IStudentSubmissionService studentSubmissionService)
    {
        _studentSubmissionService = studentSubmissionService;
    }

    private Guid CurrentStudentId => Guid.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);

    [HttpGet("submissions")]
    public async Task<IActionResult> GetMySubmissions([FromQuery] int page = 1, CancellationToken cancellationToken = default)
    {
        var result = await _studentSubmissionService.GetMySubmissionsAsync(CurrentStudentId, page, cancellationToken);
        return Ok(result);
    }

    [HttpGet("submissions/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _studentSubmissionService.GetByIdAsync(CurrentStudentId, id, cancellationToken);
        return Ok(result);
    }

    [HttpPost("assignments/{assignmentId:guid}/submit")]
    public async Task<IActionResult> Submit(Guid assignmentId, [FromBody] SubmitAssignmentRequest request, CancellationToken cancellationToken)
    {
        var result = await _studentSubmissionService.SubmitAsync(CurrentStudentId, assignmentId, request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPut("submissions/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSubmissionRequest request, CancellationToken cancellationToken)
    {
        var result = await _studentSubmissionService.UpdateAsync(CurrentStudentId, id, request, cancellationToken);
        return Ok(result);
    }
}