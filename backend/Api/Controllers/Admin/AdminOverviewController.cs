using AssignmentSubmissionSystem.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Admin;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/admin")]
public class AdminOverviewController : ControllerBase
{
    private readonly IAdminOverviewService _adminOverviewService;

    public AdminOverviewController(IAdminOverviewService adminOverviewService)
    {
        _adminOverviewService = adminOverviewService;
    }

    [HttpGet("assignments")]
    public async Task<IActionResult> GetAllAssignments([FromQuery] int page = 1, CancellationToken cancellationToken = default)
    {
        var result = await _adminOverviewService.GetAllAssignmentsAsync(page, cancellationToken);
        return Ok(result);
    }

    [HttpGet("submissions")]
    public async Task<IActionResult> GetAllSubmissions([FromQuery] int page = 1, CancellationToken cancellationToken = default)
    {
        var result = await _adminOverviewService.GetAllSubmissionsAsync(page, cancellationToken);
        return Ok(result);
    }
}