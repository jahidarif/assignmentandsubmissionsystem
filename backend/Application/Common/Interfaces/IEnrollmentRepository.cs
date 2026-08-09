using AssignmentSubmissionSystem.Domain.Entities;

namespace AssignmentSubmissionSystem.Application.Common.Interfaces;

public interface IEnrollmentRepository : IRepository<Enrollment>
{
    Task<(List<Enrollment> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, Guid? classCourseId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid studentId, Guid classCourseId, CancellationToken cancellationToken = default);
    Task<bool> IsStudentEnrolledInAssignmentClassAsync(Guid studentId, Guid assignmentId, CancellationToken cancellationToken = default);
}