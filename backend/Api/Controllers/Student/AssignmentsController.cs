using System.IdentityModel.Tokens.Jwt;
using AssignmentSubmissionSystem.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Student;

[Authorize(Roles = "Student")]
[ApiController]
[Route("api/student/assignments")]
public class AssignmentsController : ControllerBase
{
    private readonly IStudentAssignmentService _studentAssignmentService;

    public AssignmentsController(IStudentAssignmentService studentAssignmentService)
    {
        _studentAssignmentService = studentAssignmentService;
    }

    private Guid CurrentStudentId => Guid.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);

    [HttpGet]
    public async Task<IActionResult> GetAssignments([FromQuery] int page = 1, CancellationToken cancellationToken = default)
    {
        var result = await _studentAssignmentService.GetVisibleAssignmentsAsync(CurrentStudentId, page, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _studentAssignmentService.GetByIdAsync(CurrentStudentId, id, cancellationToken);
        return Ok(result);
    }
}