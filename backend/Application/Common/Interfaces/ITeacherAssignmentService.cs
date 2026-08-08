using AssignmentSubmissionSystem.Application.Common.Models;
using AssignmentSubmissionSystem.Application.Features.Admin.TeacherAssignments.Dtos;

namespace AssignmentSubmissionSystem.Application.Common.Interfaces;

public interface ITeacherAssignmentService
{
    Task<PagedResult<TeacherAssignmentDto>> GetTeacherAssignmentsAsync(int page, Guid? teacherId, CancellationToken cancellationToken = default);
    Task<TeacherAssignmentDto> CreateAsync(CreateTeacherAssignmentRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}