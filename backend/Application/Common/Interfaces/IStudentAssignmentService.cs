using AssignmentSubmissionSystem.Application.Common.Models;
using AssignmentSubmissionSystem.Application.Features.Student.Dtos;

namespace AssignmentSubmissionSystem.Application.Common.Interfaces;

public interface IStudentAssignmentService
{
    Task<PagedResult<StudentAssignmentDto>> GetVisibleAssignmentsAsync(Guid studentId, int page, CancellationToken cancellationToken = default);
    Task<StudentAssignmentDto> GetByIdAsync(Guid studentId, Guid id, CancellationToken cancellationToken = default);
}