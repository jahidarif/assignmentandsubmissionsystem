using AssignmentSubmissionSystem.Application.Common.Exceptions;
using AssignmentSubmissionSystem.Application.Common.Interfaces;
using AssignmentSubmissionSystem.Application.Common.Models;
using AssignmentSubmissionSystem.Application.Features.Teacher.Assignments.Dtos;
using AssignmentSubmissionSystem.Domain.Entities;
using AssignmentSubmissionSystem.Domain.Enums;
using FluentValidation;
using ValidationException = AssignmentSubmissionSystem.Application.Common.Exceptions.ValidationException;

namespace AssignmentSubmissionSystem.Application.Features.Teacher.Assignments;

public class AssignmentManagementService : IAssignmentManagementService
{
    private const int PageSize = 10;

    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateAssignmentRequest> _createValidator;
    private readonly IValidator<UpdateAssignmentRequest> _updateValidator;

    public AssignmentManagementService(
        IUnitOfWork unitOfWork,
        IValidator<CreateAssignmentRequest> createValidator,
        IValidator<UpdateAssignmentRequest> updateValidator)
    {
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<PagedResult<TeacherAssignmentListDto>> GetAssignmentsAsync(Guid teacherId, int page, CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;

        var (items, totalCount) = await _unitOfWork.Assignments.GetPagedByTeacherAsync(teacherId, page, PageSize, cancellationToken);

        return new PagedResult<TeacherAssignmentListDto>
        {
            Items = items.Select(MapToDto).ToList(),
            Page = page,
            PageSize = PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<TeacherAssignmentListDto> GetByIdAsync(Guid teacherId, Guid id, CancellationToken cancellationToken = default)
    {
        var assignment = await GetOwnedAssignmentAsync(teacherId, id, cancellationToken);
        return MapToDto(assignment);
    }

    public async Task<TeacherAssignmentListDto> CreateAsync(Guid teacherId, CreateAssignmentRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var isAssigned = await _unitOfWork.TeacherAssignments.ExistsAsync(teacherId, request.ClassSubjectId, cancellationToken);
        if (!isAssigned)
        {
            throw new ConflictException("You are not assigned to teach this class-subject.");
        }

        var assignment = new Assignment
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Deadline = request.Deadline,
            MaxMarks = request.MaxMarks,
            ClassSubjectId = request.ClassSubjectId,
            TeacherId = teacherId,
            Status = request.Status
        };

        await _unitOfWork.Assignments.AddAsync(assignment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var created = await _unitOfWork.Assignments.GetByIdWithDetailsAsync(assignment.Id, cancellationToken);
        return MapToDto(created!);
    }

    public async Task<TeacherAssignmentListDto> UpdateAsync(Guid teacherId, Guid id, UpdateAssignmentRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var assignment = await GetOwnedAssignmentAsync(teacherId, id, cancellationToken);

        assignment.Title = request.Title.Trim();
        assignment.Description = request.Description.Trim();
        assignment.Deadline = request.Deadline;
        assignment.MaxMarks = request.MaxMarks;

        _unitOfWork.Assignments.Update(assignment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(assignment);
    }

    public async Task DeleteAsync(Guid teacherId, Guid id, CancellationToken cancellationToken = default)
    {
        var assignment = await GetOwnedAssignmentAsync(teacherId, id, cancellationToken);

        _unitOfWork.Assignments.Remove(assignment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<TeacherAssignmentListDto> PublishAsync(Guid teacherId, Guid id, CancellationToken cancellationToken = default)
    {
        var assignment = await GetOwnedAssignmentAsync(teacherId, id, cancellationToken);

        assignment.Status = AssignmentStatus.Published;

        _unitOfWork.Assignments.Update(assignment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(assignment);
    }

    public async Task<List<TeacherClassSubjectDto>> GetClassSubjectsLookupAsync(Guid teacherId, CancellationToken cancellationToken = default)
    {
        var teacherAssignments = await _unitOfWork.TeacherAssignments.GetByTeacherIdAsync(teacherId, cancellationToken);

        return teacherAssignments.Select(ta => new TeacherClassSubjectDto
        {
            Id = ta.ClassSubjectId,
            ClassCourseName = ta.ClassSubject.ClassCourse.Name,
            SubjectName = ta.ClassSubject.Subject.Name
        }).ToList();
    }

    private async Task<Assignment> GetOwnedAssignmentAsync(Guid teacherId, Guid id, CancellationToken cancellationToken)
    {
        var assignment = await _unitOfWork.Assignments.GetByIdWithDetailsAsync(id, cancellationToken);

        if (assignment is null || assignment.TeacherId != teacherId)
        {
            throw new NotFoundException("Assignment not found.");
        }

        return assignment;
    }

    private static TeacherAssignmentListDto MapToDto(Assignment a) => new()
    {
        Id = a.Id,
        Title = a.Title,
        Description = a.Description,
        Deadline = a.Deadline,
        MaxMarks = a.MaxMarks,
        Status = a.Status.ToString(),
        ClassSubjectId = a.ClassSubjectId,
        ClassCourseName = a.ClassSubject.ClassCourse.Name,
        SubjectName = a.ClassSubject.Subject.Name
    };
}