using AssignmentSubmissionSystem.Application.Common.Models;
using AssignmentSubmissionSystem.Application.Features.Admin.Overview.Dtos;

namespace AssignmentSubmissionSystem.Application.Common.Interfaces;

public interface IAdminOverviewService
{
    Task<PagedResult<AdminAssignmentDto>> GetAllAssignmentsAsync(int page, CancellationToken cancellationToken = default);
    Task<PagedResult<AdminSubmissionDto>> GetAllSubmissionsAsync(int page, CancellationToken cancellationToken = default);
}