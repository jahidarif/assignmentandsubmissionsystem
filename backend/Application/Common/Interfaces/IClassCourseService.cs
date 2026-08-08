using AssignmentSubmissionSystem.Application.Common.Models;
using AssignmentSubmissionSystem.Application.Features.Admin.ClassCourses.Dtos;

namespace AssignmentSubmissionSystem.Application.Common.Interfaces;

public interface IClassCourseService
{
    Task<PagedResult<ClassCourseDto>> GetClassCoursesAsync(int page, CancellationToken cancellationToken = default);
    Task<ClassCourseDto> CreateAsync(CreateClassCourseRequest request, CancellationToken cancellationToken = default);
    Task<ClassCourseDto> UpdateAsync(Guid id, UpdateClassCourseRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ClassCourseDto>> GetAllLookupAsync(CancellationToken cancellationToken = default);
}