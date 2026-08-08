using AssignmentSubmissionSystem.Application.Common.Models;
using AssignmentSubmissionSystem.Application.Features.Admin.ClassSubjects.Dtos;

namespace AssignmentSubmissionSystem.Application.Common.Interfaces;

public interface IClassSubjectService
{
    Task<PagedResult<ClassSubjectDto>> GetClassSubjectsAsync(int page, Guid? classCourseId, CancellationToken cancellationToken = default);
    Task<ClassSubjectDto> CreateAsync(CreateClassSubjectRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ClassSubjectDto>> GetAllLookupAsync(CancellationToken cancellationToken = default);
}