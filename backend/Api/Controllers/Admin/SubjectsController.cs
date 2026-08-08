using AssignmentSubmissionSystem.Application.Common.Interfaces;
using AssignmentSubmissionSystem.Application.Features.Admin.Subjects.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Admin;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/admin/subjects")]
public class SubjectsController : ControllerBase
{
    private readonly ISubjectService _subjectService;

    public SubjectsController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    [HttpGet]
    public async Task<IActionResult> GetSubjects([FromQuery] int page = 1, CancellationToken cancellationToken = default)
    {
        var result = await _subjectService.GetSubjectsAsync(page, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSubjectRequest request, CancellationToken cancellationToken)
    {
        var result = await _subjectService.CreateAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSubjectRequest request, CancellationToken cancellationToken)
    {
        var result = await _subjectService.UpdateAsync(id, request, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _subjectService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet("lookup")]
    public async Task<IActionResult> GetLookup(CancellationToken cancellationToken)
    {
        var result = await _subjectService.GetAllLookupAsync(cancellationToken);
        return Ok(result);
    }
}