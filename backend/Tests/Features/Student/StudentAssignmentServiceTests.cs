using AssignmentSubmissionSystem.Application.Common.Exceptions;
using AssignmentSubmissionSystem.Application.Features.Student;
using AssignmentSubmissionSystem.Domain.Entities;
using AssignmentSubmissionSystem.Domain.Enums;
using Moq;
using Tests.Common;
using Xunit;

namespace Tests.Features.Student;

public class StudentAssignmentServiceTests
{
    private readonly MockUnitOfWorkBuilder _builder = new();

    private StudentAssignmentService CreateService()
        => new(_builder.Build().Object);

    private static Assignment BuildAssignment(Guid id, AssignmentStatus status)
        => new()
        {
            Id = id,
            Title = "Homework 1",
            Deadline = DateTime.UtcNow.AddDays(3),
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
    public async Task GetByIdAsync_WhenAssignmentIsDraft_ThrowsNotFoundException()
    {
        var studentId = Guid.NewGuid();
        var assignmentId = Guid.NewGuid();

        _builder.Assignments
            .Setup(r => r.GetByIdWithDetailsAsync(assignmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BuildAssignment(assignmentId, AssignmentStatus.Draft));

        var service = CreateService();

        await Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(studentId, assignmentId));
    }

    [Fact]
    public async Task GetByIdAsync_WhenStudentNotEnrolled_ThrowsNotFoundException()
    {
        var studentId = Guid.NewGuid();
        var assignmentId = Guid.NewGuid();

        _builder.Assignments
            .Setup(r => r.GetByIdWithDetailsAsync(assignmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BuildAssignment(assignmentId, AssignmentStatus.Published));
        _builder.Enrollments
            .Setup(r => r.IsStudentEnrolledInAssignmentClassAsync(studentId, assignmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var service = CreateService();

        await Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(studentId, assignmentId));
    }

    [Fact]
    public async Task GetByIdAsync_WhenPublishedAndEnrolled_ReturnsAssignment()
    {
        var studentId = Guid.NewGuid();
        var assignmentId = Guid.NewGuid();

        _builder.Assignments
            .Setup(r => r.GetByIdWithDetailsAsync(assignmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BuildAssignment(assignmentId, AssignmentStatus.Published));
        _builder.Enrollments
            .Setup(r => r.IsStudentEnrolledInAssignmentClassAsync(studentId, assignmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _builder.Submissions
            .Setup(r => r.GetByAssignmentAndStudentAsync(assignmentId, studentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Submission?)null);

        var service = CreateService();

        var result = await service.GetByIdAsync(studentId, assignmentId);

        Assert.Equal("Homework 1", result.Title);
        Assert.False(result.HasSubmitted);
    }
}