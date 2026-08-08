using AssignmentSubmissionSystem.Application.Common.Interfaces;
using AssignmentSubmissionSystem.Application.Features.Admin.ClassCourses.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Admin;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/admin/class-courses")]
public class ClassCoursesController : ControllerBase
{
    private readonly IClassCourseService _classCourseService;

    public ClassCoursesController(IClassCourseService classCourseService)
    {
        _classCourseService = classCourseService;
    }

    [HttpGet]
    public async Task<IActionResult> GetClassCourses([FromQuery] int page = 1, CancellationToken cancellationToken = default)
    {
        var result = await _classCourseService.GetClassCoursesAsync(page, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClassCourseRequest request, CancellationToken cancellationToken)
    {
        var result = await _classCourseService.CreateAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateClassCourseRequest request, CancellationToken cancellationToken)
    {
        var result = await _classCourseService.UpdateAsync(id, request, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _classCourseService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet("lookup")]
    public async Task<IActionResult> GetLookup(CancellationToken cancellationToken)
    {
        var result = await _classCourseService.GetAllLookupAsync(cancellationToken);
        return Ok(result);
    }
}