using System.IdentityModel.Tokens.Jwt;
using AssignmentSubmissionSystem.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Teacher;

[Authorize(Roles = "Teacher")]
[ApiController]
[Route("api/teacher/class-subjects")]
public class ClassSubjectsController : ControllerBase
{
    private readonly IAssignmentManagementService _assignmentManagementService;

    public ClassSubjectsController(IAssignmentManagementService assignmentManagementService)
    {
        _assignmentManagementService = assignmentManagementService;
    }

    private Guid CurrentTeacherId => Guid.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);

    [HttpGet]
    public async Task<IActionResult> GetLookup(CancellationToken cancellationToken)
    {
        var result = await _assignmentManagementService.GetClassSubjectsLookupAsync(CurrentTeacherId, cancellationToken);
        return Ok(result);
    }
}