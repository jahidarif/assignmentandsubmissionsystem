using AssignmentSubmissionSystem.Application.Common.Interfaces;
using AssignmentSubmissionSystem.Application.Features.Admin.TeacherAssignments.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Admin;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/admin/teacher-assignments")]
public class TeacherAssignmentsController : ControllerBase
{
    private readonly ITeacherAssignmentService _teacherAssignmentService;

    public TeacherAssignmentsController(ITeacherAssignmentService teacherAssignmentService)
    {
        _teacherAssignmentService = teacherAssignmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTeacherAssignments([FromQuery] int page = 1, [FromQuery] Guid? teacherId = null, CancellationToken cancellationToken = default)
    {
        var result = await _teacherAssignmentService.GetTeacherAssignmentsAsync(page, teacherId, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTeacherAssignmentRequest request, CancellationToken cancellationToken)
    {
        var result = await _teacherAssignmentService.CreateAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _teacherAssignmentService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}