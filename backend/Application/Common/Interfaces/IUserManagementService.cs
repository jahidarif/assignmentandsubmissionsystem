using AssignmentSubmissionSystem.Application.Common.Models;
using AssignmentSubmissionSystem.Application.Features.Admin.Users.Dtos;
using AssignmentSubmissionSystem.Domain.Enums;

namespace AssignmentSubmissionSystem.Application.Common.Interfaces;

public interface IUserManagementService
{
    Task<PagedResult<UserListItemDto>> GetUsersAsync(int page, UserRole? role, bool? isActive, CancellationToken cancellationToken = default);
    Task<UserListItemDto> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserListItemDto> UpdateUserAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task DeactivateUserAsync(Guid id, CancellationToken cancellationToken = default);
    Task ReactivateUserAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<UserListItemDto>> GetTeachersLookupAsync(CancellationToken cancellationToken = default);
    Task<List<UserListItemDto>> GetStudentsLookupAsync(CancellationToken cancellationToken = default);
}