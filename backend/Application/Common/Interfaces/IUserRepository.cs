using AssignmentSubmissionSystem.Domain.Entities;
using AssignmentSubmissionSystem.Domain.Enums;

namespace AssignmentSubmissionSystem.Application.Common.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<(List<User> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, UserRole? role, bool? isActive, CancellationToken cancellationToken = default);
    Task<List<User>> GetActiveByRoleAsync(UserRole role, CancellationToken cancellationToken = default);
}