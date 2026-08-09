using AssignmentSubmissionSystem.Application.Common.Models;
using AssignmentSubmissionSystem.Application.Features.Teacher.Assignments.Dtos;

namespace AssignmentSubmissionSystem.Application.Common.Interfaces;

public interface IAssignmentManagementService
{
    Task<PagedResult<TeacherAssignmentListDto>> GetAssignmentsAsync(Guid teacherId, int page, CancellationToken cancellationToken = default);
    Task<TeacherAssignmentListDto> GetByIdAsync(Guid teacherId, Guid id, CancellationToken cancellationToken = default);
    Task<TeacherAssignmentListDto> CreateAsync(Guid teacherId, CreateAssignmentRequest request, CancellationToken cancellationToken = default);
    Task<TeacherAssignmentListDto> UpdateAsync(Guid teacherId, Guid id, UpdateAssignmentRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid teacherId, Guid id, CancellationToken cancellationToken = default);
    Task<TeacherAssignmentListDto> PublishAsync(Guid teacherId, Guid id, CancellationToken cancellationToken = default);
    Task<List<TeacherClassSubjectDto>> GetClassSubjectsLookupAsync(Guid teacherId, CancellationToken cancellationToken = default);
}