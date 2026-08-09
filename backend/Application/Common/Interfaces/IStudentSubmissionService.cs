using AssignmentSubmissionSystem.Application.Common.Models;
using AssignmentSubmissionSystem.Application.Features.Student.Dtos;

namespace AssignmentSubmissionSystem.Application.Common.Interfaces;

public interface IStudentSubmissionService
{
    Task<PagedResult<StudentSubmissionDto>> GetMySubmissionsAsync(Guid studentId, int page, CancellationToken cancellationToken = default);
    Task<StudentSubmissionDto> GetByIdAsync(Guid studentId, Guid id, CancellationToken cancellationToken = default);
    Task<StudentSubmissionDto> SubmitAsync(Guid studentId, Guid assignmentId, SubmitAssignmentRequest request, CancellationToken cancellationToken = default);
    Task<StudentSubmissionDto> UpdateAsync(Guid studentId, Guid id, UpdateSubmissionRequest request, CancellationToken cancellationToken = default);
}