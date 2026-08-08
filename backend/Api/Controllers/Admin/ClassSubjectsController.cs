using AssignmentSubmissionSystem.Application.Common.Interfaces;
using AssignmentSubmissionSystem.Application.Features.Admin.ClassSubjects.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Admin;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/admin/class-subjects")]
public class ClassSubjectsController : ControllerBase
{
    private readonly IClassSubjectService _classSubjectService;

    public ClassSubjectsController(IClassSubjectService classSubjectService)
    {
        _classSubjectService = classSubjectService;
    }

    [HttpGet]
    public async Task<IActionResult> GetClassSubjects([FromQuery] int page = 1, [FromQuery] Guid? classCourseId = null, CancellationToken cancellationToken = default)
    {
        var result = await _classSubjectService.GetClassSubjectsAsync(page, classCourseId, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClassSubjectRequest request, CancellationToken cancellationToken)
    {
        var result = await _classSubjectService.CreateAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _classSubjectService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet("lookup")]
    public async Task<IActionResult> GetLookup(CancellationToken cancellationToken)
    {
        var result = await _classSubjectService.GetAllLookupAsync(cancellationToken);
        return Ok(result);
    }
}