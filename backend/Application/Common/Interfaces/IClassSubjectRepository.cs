using AssignmentSubmissionSystem.Domain.Entities;

namespace AssignmentSubmissionSystem.Application.Common.Interfaces;

public interface IClassSubjectRepository : IRepository<ClassSubject>
{
    Task<(List<ClassSubject> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, Guid? classCourseId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid classCourseId, Guid subjectId, CancellationToken cancellationToken = default);
    Task<List<ClassSubject>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);
}