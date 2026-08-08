using AssignmentSubmissionSystem.Application.Common.Exceptions;
using AssignmentSubmissionSystem.Application.Common.Interfaces;
using AssignmentSubmissionSystem.Application.Common.Models;
using AssignmentSubmissionSystem.Application.Features.Admin.TeacherAssignments.Dtos;
using AssignmentSubmissionSystem.Domain.Entities;
using AssignmentSubmissionSystem.Domain.Enums;
using FluentValidation;
using ValidationException = AssignmentSubmissionSystem.Application.Common.Exceptions.ValidationException;

namespace AssignmentSubmissionSystem.Application.Features.Admin.TeacherAssignments;

public class TeacherAssignmentService : ITeacherAssignmentService
{
    private const int PageSize = 10;

    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateTeacherAssignmentRequest> _createValidator;

    public TeacherAssignmentService(IUnitOfWork unitOfWork, IValidator<CreateTeacherAssignmentRequest> createValidator)
    {
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
    }

    public async Task<PagedResult<TeacherAssignmentDto>> GetTeacherAssignmentsAsync(int page, Guid? teacherId, CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;

        var (items, totalCount) = await _unitOfWork.TeacherAssignments.GetPagedAsync(page, PageSize, teacherId, cancellationToken);

        return new PagedResult<TeacherAssignmentDto>
        {
            Items = items.Select(MapToDto).ToList(),
            Page = page,
            PageSize = PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<TeacherAssignmentDto> CreateAsync(CreateTeacherAssignmentRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var teacher = await _unitOfWork.Users.GetByIdAsync(request.TeacherId, cancellationToken)
            ?? throw new NotFoundException("Teacher not found.");

        if (teacher.Role != UserRole.Teacher)
        {
            throw new ConflictException("Selected user is not a Teacher.");
        }

        var classSubject = await _unitOfWork.ClassSubjects.GetByIdAsync(request.ClassSubjectId, cancellationToken)
            ?? throw new NotFoundException("Class-subject not found.");

        var alreadyExists = await _unitOfWork.TeacherAssignments.ExistsAsync(request.TeacherId, request.ClassSubjectId, cancellationToken);
        if (alreadyExists)
        {
            throw new ConflictException("This teacher is already assigned to this class-subject.");
        }

        var teacherAssignment = new TeacherAssignment
        {
            TeacherId = request.TeacherId,
            ClassSubjectId = request.ClassSubjectId
        };

        await _unitOfWork.TeacherAssignments.AddAsync(teacherAssignment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new TeacherAssignmentDto
        {
            Id = teacherAssignment.Id,
            TeacherId = teacher.Id,
            TeacherName = teacher.FullName,
            TeacherEmail = teacher.Email,
            ClassSubjectId = classSubject.Id,
            ClassCourseName = classSubject.ClassCourse.Name,
            SubjectName = classSubject.Subject.Name
        };
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var teacherAssignment = await _unitOfWork.TeacherAssignments.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Teacher assignment not found.");

        _unitOfWork.TeacherAssignments.Remove(teacherAssignment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static TeacherAssignmentDto MapToDto(TeacherAssignment ta) => new()
    {
        Id = ta.Id,
        TeacherId = ta.TeacherId,
        TeacherName = ta.Teacher.FullName,
        TeacherEmail = ta.Teacher.Email,
        ClassSubjectId = ta.ClassSubjectId,
        ClassCourseName = ta.ClassSubject.ClassCourse.Name,
        SubjectName = ta.ClassSubject.Subject.Name
    };
}