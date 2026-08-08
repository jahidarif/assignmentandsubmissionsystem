using AssignmentSubmissionSystem.Application.Common.Exceptions;
using AssignmentSubmissionSystem.Application.Common.Interfaces;
using AssignmentSubmissionSystem.Application.Common.Models;
using AssignmentSubmissionSystem.Application.Features.Admin.Enrollments.Dtos;
using AssignmentSubmissionSystem.Domain.Entities;
using AssignmentSubmissionSystem.Domain.Enums;
using FluentValidation;
using ValidationException = AssignmentSubmissionSystem.Application.Common.Exceptions.ValidationException;

namespace AssignmentSubmissionSystem.Application.Features.Admin.Enrollments;

public class EnrollmentService : IEnrollmentService
{
    private const int PageSize = 10;

    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateEnrollmentRequest> _createValidator;

    public EnrollmentService(IUnitOfWork unitOfWork, IValidator<CreateEnrollmentRequest> createValidator)
    {
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
    }

    public async Task<PagedResult<EnrollmentDto>> GetEnrollmentsAsync(int page, Guid? classCourseId, CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;

        var (items, totalCount) = await _unitOfWork.Enrollments.GetPagedAsync(page, PageSize, classCourseId, cancellationToken);

        return new PagedResult<EnrollmentDto>
        {
            Items = items.Select(MapToDto).ToList(),
            Page = page,
            PageSize = PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<EnrollmentDto> CreateAsync(CreateEnrollmentRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var student = await _unitOfWork.Users.GetByIdAsync(request.StudentId, cancellationToken)
            ?? throw new NotFoundException("Student not found.");

        if (student.Role != UserRole.Student)
        {
            throw new ConflictException("Selected user is not a Student.");
        }

        var classCourse = await _unitOfWork.ClassCourses.GetByIdAsync(request.ClassCourseId, cancellationToken)
            ?? throw new NotFoundException("Class not found.");

        var alreadyExists = await _unitOfWork.Enrollments.ExistsAsync(request.StudentId, request.ClassCourseId, cancellationToken);
        if (alreadyExists)
        {
            throw new ConflictException("This student is already enrolled in this class.");
        }

        var enrollment = new Enrollment
        {
            StudentId = request.StudentId,
            ClassCourseId = request.ClassCourseId
        };

        await _unitOfWork.Enrollments.AddAsync(enrollment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new EnrollmentDto
        {
            Id = enrollment.Id,
            StudentId = student.Id,
            StudentName = student.FullName,
            StudentEmail = student.Email,
            ClassCourseId = classCourse.Id,
            ClassCourseName = classCourse.Name,
            EnrolledAt = enrollment.EnrolledAt
        };
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var enrollment = await _unitOfWork.Enrollments.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Enrollment not found.");

        _unitOfWork.Enrollments.Remove(enrollment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static EnrollmentDto MapToDto(Enrollment e) => new()
    {
        Id = e.Id,
        StudentId = e.StudentId,
        StudentName = e.Student.FullName,
        StudentEmail = e.Student.Email,
        ClassCourseId = e.ClassCourseId,
        ClassCourseName = e.ClassCourse.Name,
        EnrolledAt = e.EnrolledAt
    };
}