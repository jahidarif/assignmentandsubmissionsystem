using AssignmentSubmissionSystem.Application.Common.Exceptions;
using AssignmentSubmissionSystem.Application.Features.Teacher.Assignments;
using AssignmentSubmissionSystem.Application.Features.Teacher.Assignments.Dtos;
using AssignmentSubmissionSystem.Application.Features.Teacher.Assignments.Validators;
using AssignmentSubmissionSystem.Domain.Entities;
using AssignmentSubmissionSystem.Domain.Enums;
using Moq;
using Tests.Common;
using Xunit;

namespace Tests.Features.Teacher;

public class AssignmentManagementServiceTests
{
    private readonly MockUnitOfWorkBuilder _builder = new();

    private AssignmentManagementService CreateService()
        => new(_builder.Build().Object, new CreateAssignmentRequestValidator(), new UpdateAssignmentRequestValidator());

    [Fact]
    public async Task CreateAsync_WhenTeacherNotAssignedToClassSubject_ThrowsConflictException()
    {
        var teacherId = Guid.NewGuid();
        var classSubjectId = Guid.NewGuid();

        _builder.TeacherAssignments
            .Setup(r => r.ExistsAsync(teacherId, classSubjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var service = CreateService();

        var request = new CreateAssignmentRequest
        {
            Title = "Homework 1",
            Description = "Do the exercises",
            Deadline = DateTime.UtcNow.AddDays(3),
            MaxMarks = 100,
            ClassSubjectId = classSubjectId,
            Status = AssignmentStatus.Draft
        };

        await Assert.ThrowsAsync<ConflictException>(() => service.CreateAsync(teacherId, request));
    }

    [Fact]
    public async Task CreateAsync_WhenTeacherIsAssigned_CreatesAssignment()
    {
        var teacherId = Guid.NewGuid();
        var classSubjectId = Guid.NewGuid();
        var assignmentId = Guid.NewGuid();

        var classSubject = new ClassSubject
        {
            Id = classSubjectId,
            ClassCourse = new ClassCourse { Name = "Grade 10" },
            Subject = new Subject { Name = "Math", Code = "MATH101" }
        };

        _builder.TeacherAssignments
            .Setup(r => r.ExistsAsync(teacherId, classSubjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _builder.Assignments
            .Setup(r => r.AddAsync(It.IsAny<Assignment>(), It.IsAny<CancellationToken>()))
            .Callback<Assignment, CancellationToken>((a, _) => a.Id = assignmentId)
            .Returns(Task.CompletedTask);

        _builder.Assignments
            .Setup(r => r.GetByIdWithDetailsAsync(assignmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Assignment
            {
                Id = assignmentId,
                Title = "Homework 1",
                Description = "Do the exercises",
                Deadline = DateTime.UtcNow.AddDays(3),
                MaxMarks = 100,
                ClassSubjectId = classSubjectId,
                ClassSubject = classSubject,
                TeacherId = teacherId,
                Status = AssignmentStatus.Draft
            });

        var service = CreateService();

        var request = new CreateAssignmentRequest
        {
            Title = "Homework 1",
            Description = "Do the exercises",
            Deadline = DateTime.UtcNow.AddDays(3),
            MaxMarks = 100,
            ClassSubjectId = classSubjectId,
            Status = AssignmentStatus.Draft
        };

        var result = await service.CreateAsync(teacherId, request);

        Assert.Equal("Homework 1", result.Title);
        Assert.Equal("Grade 10", result.ClassCourseName);
        _builder.Assignments.Verify(r => r.AddAsync(It.IsAny<Assignment>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenAssignmentBelongsToDifferentTeacher_ThrowsNotFoundException()
    {
        var teacherId = Guid.NewGuid();
        var otherTeacherId = Guid.NewGuid();
        var assignmentId = Guid.NewGuid();

        _builder.Assignments
            .Setup(r => r.GetByIdWithDetailsAsync(assignmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Assignment
            {
                Id = assignmentId,
                TeacherId = otherTeacherId,
                ClassSubject = new ClassSubject
                {
                    ClassCourse = new ClassCourse { Name = "Grade 10" },
                    Subject = new Subject { Name = "Math", Code = "MATH101" }
                }
            });

        var service = CreateService();

        await Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(teacherId, assignmentId));
    }
}