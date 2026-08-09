using AssignmentSubmissionSystem.Application.Common.Models;
using AssignmentSubmissionSystem.Application.Features.Teacher.Submissions.Dtos;

namespace AssignmentSubmissionSystem.Application.Common.Interfaces;

public interface ISubmissionGradingService
{
    Task<PagedResult<TeacherSubmissionDto>> GetSubmissionsForAssignmentAsync(Guid teacherId, Guid assignmentId, int page, CancellationToken cancellationToken = default);
    Task<TeacherSubmissionDto> GetByIdAsync(Guid teacherId, Guid id, CancellationToken cancellationToken = default);
    Task<TeacherSubmissionDto> GradeAsync(Guid teacherId, Guid id, GradeSubmissionRequest request, CancellationToken cancellationToken = default);
    Task<TeacherSubmissionDto> UpdateStatusAsync(Guid teacherId, Guid id, UpdateSubmissionStatusRequest request, CancellationToken cancellationToken = default);
}