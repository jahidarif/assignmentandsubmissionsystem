using AssignmentSubmissionSystem.Application.Common.Exceptions;
using AssignmentSubmissionSystem.Application.Common.Interfaces;
using AssignmentSubmissionSystem.Application.Common.Models;
using AssignmentSubmissionSystem.Application.Features.Student.Dtos;
using AssignmentSubmissionSystem.Domain.Entities;
using AssignmentSubmissionSystem.Domain.Enums;
using FluentValidation;
using ValidationException = AssignmentSubmissionSystem.Application.Common.Exceptions.ValidationException;

namespace AssignmentSubmissionSystem.Application.Features.Student;

public class StudentSubmissionService : IStudentSubmissionService
{
    private const int PageSize = 10;

    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<SubmitAssignmentRequest> _submitValidator;
    private readonly IValidator<UpdateSubmissionRequest> _updateValidator;

    public StudentSubmissionService(
        IUnitOfWork unitOfWork,
        IValidator<SubmitAssignmentRequest> submitValidator,
        IValidator<UpdateSubmissionRequest> updateValidator)
    {
        _unitOfWork = unitOfWork;
        _submitValidator = submitValidator;
        _updateValidator = updateValidator;
    }

    public async Task<PagedResult<StudentSubmissionDto>> GetMySubmissionsAsync(Guid studentId, int page, CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;

        var (items, totalCount) = await _unitOfWork.Submissions.GetPagedByStudentAsync(studentId, page, PageSize, cancellationToken);

        return new PagedResult<StudentSubmissionDto>
        {
            Items = items.Select(MapToDto).ToList(),
            Page = page,
            PageSize = PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<StudentSubmissionDto> GetByIdAsync(Guid studentId, Guid id, CancellationToken cancellationToken = default)
    {
        var submission = await GetOwnedSubmissionAsync(studentId, id, cancellationToken);
        return MapToDto(submission);
    }

    public async Task<StudentSubmissionDto> SubmitAsync(Guid studentId, Guid assignmentId, SubmitAssignmentRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _submitValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var assignment = await _unitOfWork.Assignments.GetByIdWithDetailsAsync(assignmentId, cancellationToken);
        if (assignment is null || assignment.Status != AssignmentStatus.Published)
        {
            throw new NotFoundException("Assignment not found.");
        }

        var isEnrolled = await _unitOfWork.Enrollments.IsStudentEnrolledInAssignmentClassAsync(studentId, assignmentId, cancellationToken);
        if (!isEnrolled)
        {
            throw new NotFoundException("Assignment not found.");
        }

        if (DateTime.UtcNow > assignment.Deadline)
        {
            throw new ConflictException("The deadline for this assignment has passed.");
        }

        var existing = await _unitOfWork.Submissions.GetByAssignmentAndStudentAsync(assignmentId, studentId, cancellationToken);
        if (existing is not null)
        {
            throw new ConflictException("You have already submitted this assignment. Use update instead.");
        }

        var submission = new Submission
        {
            AssignmentId = assignmentId,
            StudentId = studentId,
            AnswerText = request.AnswerText.Trim(),
            AttachmentUrl = request.AttachmentUrl?.Trim(),
            Status = SubmissionStatus.Submitted
        };

        await _unitOfWork.Submissions.AddAsync(submission, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var created = await _unitOfWork.Submissions.GetByIdWithDetailsAsync(submission.Id, cancellationToken);
        return MapToDto(created!);
    }

    public async Task<StudentSubmissionDto> UpdateAsync(Guid studentId, Guid id, UpdateSubmissionRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var submission = await GetOwnedSubmissionAsync(studentId, id, cancellationToken);

        if (DateTime.UtcNow > submission.Assignment.Deadline)
        {
            throw new ConflictException("The deadline for this assignment has passed. You can no longer update your submission.");
        }

        submission.AnswerText = request.AnswerText.Trim();
        submission.AttachmentUrl = request.AttachmentUrl?.Trim();

        _unitOfWork.Submissions.Update(submission);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(submission);
    }

    private async Task<Submission> GetOwnedSubmissionAsync(Guid studentId, Guid id, CancellationToken cancellationToken)
    {
        var submission = await _unitOfWork.Submissions.GetByIdWithDetailsAsync(id, cancellationToken);

        if (submission is null || submission.StudentId != studentId)
        {
            throw new NotFoundException("Submission not found.");
        }

        return submission;
    }

    private static StudentSubmissionDto MapToDto(Submission s) => new()
    {
        Id = s.Id,
        AssignmentId = s.AssignmentId,
        AssignmentTitle = s.Assignment.Title,
        AssignmentDeadline = s.Assignment.Deadline,
        AssignmentMaxMarks = s.Assignment.MaxMarks,
        AnswerText = s.AnswerText,
        AttachmentUrl = s.AttachmentUrl,
        SubmittedAt = s.SubmittedAt,
        Status = s.Status.ToString(),
        Marks = s.Marks,
        Feedback = s.Feedback,
        GradedAt = s.GradedAt,
        CanUpdate = DateTime.UtcNow <= s.Assignment.Deadline
    };
}