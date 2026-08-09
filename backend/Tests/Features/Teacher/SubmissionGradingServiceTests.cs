using AssignmentSubmissionSystem.Application.Common.Exceptions;
using AssignmentSubmissionSystem.Application.Features.Teacher.Submissions;
using AssignmentSubmissionSystem.Application.Features.Teacher.Submissions.Dtos;
using AssignmentSubmissionSystem.Application.Features.Teacher.Submissions.Validators;
using AssignmentSubmissionSystem.Domain.Entities;
using AssignmentSubmissionSystem.Domain.Enums;
using Moq;
using Tests.Common;
using Xunit;

namespace Tests.Features.Teacher;

public class SubmissionGradingServiceTests
{
    private readonly MockUnitOfWorkBuilder _builder = new();

    private SubmissionGradingService CreateService()
        => new(_builder.Build().Object, new GradeSubmissionRequestValidator(), new UpdateSubmissionStatusRequestValidator());

    private static Submission BuildSubmission(Guid id, Guid teacherId, int maxMarks)
        => new()
        {
            Id = id,
            Student = new User { FullName = "John Student", Email = "john@test.com" },
            Assignment = new Assignment
            {
                Title = "Homework 1",
                MaxMarks = maxMarks,
                TeacherId = teacherId
            }
        };

    [Fact]
    public async Task GradeAsync_WhenMarksExceedMaxMarks_ThrowsConflictException()
    {
        var teacherId = Guid.NewGuid();
        var submissionId = Guid.NewGuid();

        var submission = BuildSubmission(submissionId, teacherId, maxMarks: 50);

        _builder.Submissions
            .Setup(r => r.GetByIdWithDetailsAsync(submissionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(submission);

        var service = CreateService();

        var request = new GradeSubmissionRequest { Marks = 75, Feedback = "Good effort" };

        await Assert.ThrowsAsync<ConflictException>(() => service.GradeAsync(teacherId, submissionId, request));
    }

    [Fact]
    public async Task GradeAsync_WhenSubmissionBelongsToDifferentTeacher_ThrowsNotFoundException()
    {
        var teacherId = Guid.NewGuid();
        var otherTeacherId = Guid.NewGuid();
        var submissionId = Guid.NewGuid();

        var submission = BuildSubmission(submissionId, otherTeacherId, maxMarks: 100);

        _builder.Submissions
            .Setup(r => r.GetByIdWithDetailsAsync(submissionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(submission);

        var service = CreateService();

        var request = new GradeSubmissionRequest { Marks = 50 };

        await Assert.ThrowsAsync<NotFoundException>(() => service.GradeAsync(teacherId, submissionId, request));
    }

    [Fact]
    public async Task GradeAsync_WithValidMarks_SetsGradedStatusAndMarks()
    {
        var teacherId = Guid.NewGuid();
        var submissionId = Guid.NewGuid();

        var submission = BuildSubmission(submissionId, teacherId, maxMarks: 100);

        _builder.Submissions
            .Setup(r => r.GetByIdWithDetailsAsync(submissionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(submission);

        var service = CreateService();

        var request = new GradeSubmissionRequest { Marks = 85, Feedback = "Well done" };

        var result = await service.GradeAsync(teacherId, submissionId, request);

        Assert.Equal(85, result.Marks);
        Assert.Equal("Graded", result.Status);
        Assert.Equal("Well done", result.Feedback);
        _builder.Submissions.Verify(r => r.Update(It.Is<Submission>(s => s.Status == SubmissionStatus.Graded && s.Marks == 85)), Times.Once);
    }
}