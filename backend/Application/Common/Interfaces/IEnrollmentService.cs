using AssignmentSubmissionSystem.Application.Common.Models;
using AssignmentSubmissionSystem.Application.Features.Admin.Enrollments.Dtos;

namespace AssignmentSubmissionSystem.Application.Common.Interfaces;

public interface IEnrollmentService
{
    Task<PagedResult<EnrollmentDto>> GetEnrollmentsAsync(int page, Guid? classCourseId, CancellationToken cancellationToken = default);
    Task<EnrollmentDto> CreateAsync(CreateEnrollmentRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}