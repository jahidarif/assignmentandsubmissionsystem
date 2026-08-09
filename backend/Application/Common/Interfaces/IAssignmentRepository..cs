using AssignmentSubmissionSystem.Domain.Entities;

namespace AssignmentSubmissionSystem.Application.Common.Interfaces;

public interface IAssignmentRepository : IRepository<Assignment>
{
    Task<(List<Assignment> Items, int TotalCount)> GetPagedByTeacherAsync(Guid teacherId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Assignment?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(List<Assignment> Items, int TotalCount)> GetPagedVisibleToStudentAsync(Guid studentId, int page, int pageSize, CancellationToken cancellationToken = default);
}