using AssignmentSubmissionSystem.Application.Common.Exceptions;
using AssignmentSubmissionSystem.Application.Features.Student;
using AssignmentSubmissionSystem.Application.Features.Student.Dtos;
using AssignmentSubmissionSystem.Application.Features.Student.Validators;
using AssignmentSubmissionSystem.Domain.Entities;
using AssignmentSubmissionSystem.Domain.Enums;
using Moq;
using Tests.Common;
using Xunit;

namespace Tests.Features.Student;

public class StudentSubmissionServiceTests
{
    private readonly MockUnitOfWorkBuilder _builder = new();

    private StudentSubmissionService CreateService()
        => new(_builder.Build().Object, new SubmitAssignmentRequestValidator(), new UpdateSubmissionRequestValidator());

    private static Assignment BuildAssignment(Guid id, DateTime deadline, AssignmentStatus status = AssignmentStatus.Published)
        => new()
        {
            Id = id,
            Title = "Homework 1",
            Deadline = deadline,
            MaxMarks = 100,
            Status = status,
            ClassSubject = new ClassSubject
            {
                ClassCourse = new ClassCourse { Name = "Grade 10" },
                Subject = new Subject { Name = "Math", Code = "MATH101" }
            },
            Teacher = new User { FullName = "Ms. Smith" }
        };

    [Fact]
    public async Task SubmitAsync_AfterDeadline_ThrowsConflictException()
    {
        var studentId = Guid.NewGuid();
        var assignmentId = Guid.NewGuid();

        var assignment = BuildAssignment(assignmentId, DateTime.UtcNow.AddDays(-1));

        _builder.Assignments
            .Setup(r => r.GetByIdWithDetailsAsync(assignmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assignment);

        _builder.Enrollments
            .Setup(r => r.IsStudentEnrolledInAssignmentClassAsync(studentId, assignmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = CreateService();

        var request = new SubmitAssignmentRequest { AnswerText = "My answer" };

        await Assert.ThrowsAsync<ConflictException>(() => service.SubmitAsync(studentId, assignmentId, request));
    }

    [Fact]
    public async Task SubmitAsync_WhenNotEnrolled_ThrowsNotFoundException()
    {
        var studentId = Guid.NewGuid();
        var assignmentId = Guid.NewGuid();

        var assignment = BuildAssignment(assignmentId, DateTime.UtcNow.AddDays(3));

        _builder.Assignments
            .Setup(r => r.GetByIdWithDetailsAsync(assignmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assignment);

        _builder.Enrollments
            .Setup(r => r.IsStudentEnrolledInAssignmentClassAsync(studentId, assignmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var service = CreateService();

        var request = new SubmitAssignmentRequest { AnswerText = "My answer" };

        await Assert.ThrowsAsync<NotFoundException>(() => service.SubmitAsync(studentId, assignmentId, request));
    }

    [Fact]
    public async Task SubmitAsync_WhenAlreadySubmitted_ThrowsConflictException()
    {
        var studentId = Guid.NewGuid();
        var assignmentId = Guid.NewGuid();

        var assignment = BuildAssignment(assignmentId, DateTime.UtcNow.AddDays(3));

        _builder.Assignments
            .Setup(r => r.GetByIdWithDetailsAsync(assignmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assignment);

        _builder.Enrollments
            .Setup(r => r.IsStudentEnrolledInAssignmentClassAsync(studentId, assignmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _builder.Submissions
            .Setup(r => r.GetByAssignmentAndStudentAsync(assignmentId, studentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Submission { Id = Guid.NewGuid() });

        var service = CreateService();

        var request = new SubmitAssignmentRequest { AnswerText = "My answer" };

        await Assert.ThrowsAsync<ConflictException>(() => service.SubmitAsync(studentId, assignmentId, request));
    }

    [Fact]
    public async Task UpdateAsync_AfterDeadline_ThrowsConflictException()
    {
        var studentId = Guid.NewGuid();
        var submissionId = Guid.NewGuid();

        var submission = new Submission
        {
            Id = submissionId,
            StudentId = studentId,
            Assignment = BuildAssignment(Guid.NewGuid(), DateTime.UtcNow.AddDays(-1))
        };

        _builder.Submissions
            .Setup(r => r.GetByIdWithDetailsAsync(submissionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(submission);

        var service = CreateService();

        var request = new UpdateSubmissionRequest { AnswerText = "Updated answer" };

        await Assert.ThrowsAsync<ConflictException>(() => service.UpdateAsync(studentId, submissionId, request));
    }

    [Fact]
    public async Task UpdateAsync_WhenSubmissionBelongsToDifferentStudent_ThrowsNotFoundException()
    {
        var studentId = Guid.NewGuid();
        var otherStudentId = Guid.NewGuid();
        var submissionId = Guid.NewGuid();

        var submission = new Submission
        {
            Id = submissionId,
            StudentId = otherStudentId,
            Assignment = BuildAssignment(Guid.NewGuid(), DateTime.UtcNow.AddDays(3))
        };

        _builder.Submissions
            .Setup(r => r.GetByIdWithDetailsAsync(submissionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(submission);

        var service = CreateService();

        var request = new UpdateSubmissionRequest { AnswerText = "Updated answer" };

        await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateAsync(studentId, submissionId, request));
    }
}