using AssignmentSubmissionSystem.Application.Common.Interfaces;
using AssignmentSubmissionSystem.Application.Features.Admin.Users.Dtos;
using AssignmentSubmissionSystem.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Admin;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/admin/users")]
public class UsersController : ControllerBase
{
    private readonly IUserManagementService _userManagementService;

    public UsersController(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] UserRole? role = null, [FromQuery] bool? isActive = null, CancellationToken cancellationToken = default)
    {
        var result = await _userManagementService.GetUsersAsync(page, role, isActive, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _userManagementService.GetUserByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var result = await _userManagementService.UpdateUserAsync(id, request, cancellationToken);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        await _userManagementService.DeactivateUserAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id:guid}/reactivate")]
    public async Task<IActionResult> Reactivate(Guid id, CancellationToken cancellationToken)
    {
        await _userManagementService.ReactivateUserAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet("lookup/teachers")]
    public async Task<IActionResult> GetTeachersLookup(CancellationToken cancellationToken)
    {
        var result = await _userManagementService.GetTeachersLookupAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("lookup/students")]
    public async Task<IActionResult> GetStudentsLookup(CancellationToken cancellationToken)
    {
        var result = await _userManagementService.GetStudentsLookupAsync(cancellationToken);
        return Ok(result);
    }
}