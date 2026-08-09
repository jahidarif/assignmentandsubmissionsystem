using AssignmentSubmissionSystem.Domain.Entities;

namespace AssignmentSubmissionSystem.Application.Common.Interfaces;

public interface ITeacherAssignmentRepository : IRepository<TeacherAssignment>
{
    Task<(List<TeacherAssignment> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, Guid? teacherId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid teacherId, Guid classSubjectId, CancellationToken cancellationToken = default);
    Task<List<TeacherAssignment>> GetByTeacherIdAsync(Guid teacherId, CancellationToken cancellationToken = default);
}