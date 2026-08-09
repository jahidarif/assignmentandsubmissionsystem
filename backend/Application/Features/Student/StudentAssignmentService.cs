using AssignmentSubmissionSystem.Application.Common.Exceptions;
using AssignmentSubmissionSystem.Application.Common.Interfaces;
using AssignmentSubmissionSystem.Application.Common.Models;
using AssignmentSubmissionSystem.Application.Features.Student.Dtos;
using AssignmentSubmissionSystem.Domain.Entities;
using AssignmentSubmissionSystem.Domain.Enums;

namespace AssignmentSubmissionSystem.Application.Features.Student;

public class StudentAssignmentService : IStudentAssignmentService
{
    private const int PageSize = 10;

    private readonly IUnitOfWork _unitOfWork;

    public StudentAssignmentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<StudentAssignmentDto>> GetVisibleAssignmentsAsync(Guid studentId, int page, CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;

        var (items, totalCount) = await _unitOfWork.Assignments.GetPagedVisibleToStudentAsync(studentId, page, PageSize, cancellationToken);

        var dtos = new List<StudentAssignmentDto>();
        foreach (var assignment in items)
        {
            dtos.Add(await MapToDtoAsync(assignment, studentId, cancellationToken));
        }

        return new PagedResult<StudentAssignmentDto>
        {
            Items = dtos,
            Page = page,
            PageSize = PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<StudentAssignmentDto> GetByIdAsync(Guid studentId, Guid id, CancellationToken cancellationToken = default)
    {
        var assignment = await _unitOfWork.Assignments.GetByIdWithDetailsAsync(id, cancellationToken);

        if (assignment is null || assignment.Status != AssignmentStatus.Published)
        {
            throw new NotFoundException("Assignment not found.");
        }

        var isEnrolled = await _unitOfWork.Enrollments.IsStudentEnrolledInAssignmentClassAsync(studentId, id, cancellationToken);
        if (!isEnrolled)
        {
            throw new NotFoundException("Assignment not found.");
        }

        return await MapToDtoAsync(assignment, studentId, cancellationToken);
    }

    private async Task<StudentAssignmentDto> MapToDtoAsync(Assignment assignment, Guid studentId, CancellationToken cancellationToken)
    {
        var submission = await _unitOfWork.Submissions.GetByAssignmentAndStudentAsync(assignment.Id, studentId, cancellationToken);

        return new StudentAssignmentDto
        {
            Id = assignment.Id,
            Title = assignment.Title,
            Description = assignment.Description,
            Deadline = assignment.Deadline,
            MaxMarks = assignment.MaxMarks,
            ClassCourseName = assignment.ClassSubject.ClassCourse.Name,
            SubjectName = assignment.ClassSubject.Subject.Name,
            TeacherName = assignment.Teacher.FullName,
            HasSubmitted = submission is not null,
            IsPastDeadline = DateTime.UtcNow > assignment.Deadline
        };
    }
}