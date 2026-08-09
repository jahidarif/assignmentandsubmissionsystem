using AssignmentSubmissionSystem.Application.Common.Exceptions;
using AssignmentSubmissionSystem.Application.Common.Interfaces;
using AssignmentSubmissionSystem.Application.Common.Models;
using AssignmentSubmissionSystem.Application.Features.Teacher.Submissions.Dtos;
using AssignmentSubmissionSystem.Domain.Entities;
using AssignmentSubmissionSystem.Domain.Enums;
using FluentValidation;
using ValidationException = AssignmentSubmissionSystem.Application.Common.Exceptions.ValidationException;

namespace AssignmentSubmissionSystem.Application.Features.Teacher.Submissions;

public class SubmissionGradingService : ISubmissionGradingService
{
    private const int PageSize = 10;

    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<GradeSubmissionRequest> _gradeValidator;
    private readonly IValidator<UpdateSubmissionStatusRequest> _statusValidator;

    public SubmissionGradingService(
        IUnitOfWork unitOfWork,
        IValidator<GradeSubmissionRequest> gradeValidator,
        IValidator<UpdateSubmissionStatusRequest> statusValidator)
    {
        _unitOfWork = unitOfWork;
        _gradeValidator = gradeValidator;
        _statusValidator = statusValidator;
    }

    public async Task<PagedResult<TeacherSubmissionDto>> GetSubmissionsForAssignmentAsync(Guid teacherId, Guid assignmentId, int page, CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;

        var assignment = await _unitOfWork.Assignments.GetByIdAsync(assignmentId, cancellationToken);
        if (assignment is null || assignment.TeacherId != teacherId)
        {
            throw new NotFoundException("Assignment not found.");
        }

        var (items, totalCount) = await _unitOfWork.Submissions.GetPagedByAssignmentAsync(assignmentId, page, PageSize, cancellationToken);

        return new PagedResult<TeacherSubmissionDto>
        {
            Items = items.Select(s => MapToDto(s, assignment.Title)).ToList(),
            Page = page,
            PageSize = PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<TeacherSubmissionDto> GetByIdAsync(Guid teacherId, Guid id, CancellationToken cancellationToken = default)
    {
        var submission = await GetOwnedSubmissionAsync(teacherId, id, cancellationToken);
        return MapToDto(submission, submission.Assignment.Title);
    }

    public async Task<TeacherSubmissionDto> GradeAsync(Guid teacherId, Guid id, GradeSubmissionRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _gradeValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var submission = await GetOwnedSubmissionAsync(teacherId, id, cancellationToken);

        if (request.Marks > submission.Assignment.MaxMarks)
        {
            throw new ConflictException($"Marks cannot exceed the assignment's maximum of {submission.Assignment.MaxMarks}.");
        }

        submission.Marks = request.Marks;
        submission.Feedback = request.Feedback?.Trim();
        submission.GradedByTeacherId = teacherId;
        submission.GradedAt = DateTime.UtcNow;
        submission.Status = SubmissionStatus.Graded;

        _unitOfWork.Submissions.Update(submission);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(submission, submission.Assignment.Title);
    }

    public async Task<TeacherSubmissionDto> UpdateStatusAsync(Guid teacherId, Guid id, UpdateSubmissionStatusRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _statusValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var submission = await GetOwnedSubmissionAsync(teacherId, id, cancellationToken);

        submission.Status = request.Status;

        _unitOfWork.Submissions.Update(submission);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(submission, submission.Assignment.Title);
    }

    private async Task<Submission> GetOwnedSubmissionAsync(Guid teacherId, Guid id, CancellationToken cancellationToken)
    {
        var submission = await _unitOfWork.Submissions.GetByIdWithDetailsAsync(id, cancellationToken);

        if (submission is null || submission.Assignment.TeacherId != teacherId)
        {
            throw new NotFoundException("Submission not found.");
        }

        return submission;
    }

    private static TeacherSubmissionDto MapToDto(Submission s, string assignmentTitle) => new()
    {
        Id = s.Id,
        AssignmentId = s.AssignmentId,
        AssignmentTitle = assignmentTitle,
        StudentId = s.StudentId,
        StudentName = s.Student.FullName,
        StudentEmail = s.Student.Email,
        AnswerText = s.AnswerText,
        AttachmentUrl = s.AttachmentUrl,
        SubmittedAt = s.SubmittedAt,
        Status = s.Status.ToString(),
        Marks = s.Marks,
        Feedback = s.Feedback,
        GradedAt = s.GradedAt
    };
}