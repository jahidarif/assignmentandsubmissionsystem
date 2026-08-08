using AssignmentSubmissionSystem.Application.Common.Models;
using AssignmentSubmissionSystem.Application.Features.Admin.Subjects.Dtos;

namespace AssignmentSubmissionSystem.Application.Common.Interfaces;

public interface ISubjectService
{
    Task<PagedResult<SubjectDto>> GetSubjectsAsync(int page, CancellationToken cancellationToken = default);
    Task<SubjectDto> CreateAsync(CreateSubjectRequest request, CancellationToken cancellationToken = default);
    Task<SubjectDto> UpdateAsync(Guid id, UpdateSubjectRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<SubjectDto>> GetAllLookupAsync(CancellationToken cancellationToken = default);
}