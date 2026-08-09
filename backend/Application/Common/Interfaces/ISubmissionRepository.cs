using AssignmentSubmissionSystem.Domain.Entities;

namespace AssignmentSubmissionSystem.Application.Common.Interfaces;

public interface ISubmissionRepository : IRepository<Submission>
{
    Task<(List<Submission> Items, int TotalCount)> GetPagedByAssignmentAsync(Guid assignmentId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Submission?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
}