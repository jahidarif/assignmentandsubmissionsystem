using AssignmentSubmissionSystem.Application.Common.Exceptions;
using AssignmentSubmissionSystem.Application.Features.Admin.TeacherAssignments;
using AssignmentSubmissionSystem.Application.Features.Admin.TeacherAssignments.Dtos;
using AssignmentSubmissionSystem.Application.Features.Admin.TeacherAssignments.Validators;
using AssignmentSubmissionSystem.Domain.Entities;
using AssignmentSubmissionSystem.Domain.Enums;
using Moq;
using Tests.Common;
using Xunit;

namespace Tests.Features.Admin;

public class TeacherAssignmentServiceTests
{
    private readonly MockUnitOfWorkBuilder _builder = new();

    private TeacherAssignmentService CreateService()
        => new(_builder.Build().Object, new CreateTeacherAssignmentRequestValidator());

    [Fact]
    public async Task CreateAsync_WhenUserIsNotATeacher_ThrowsConflictException()
    {
        var studentUserId = Guid.NewGuid();
        var classSubjectId = Guid.NewGuid();

        _builder.Users.Setup(r => r.GetByIdAsync(studentUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = studentUserId, Role = UserRole.Student, FullName = "Not A Teacher" });

        var service = CreateService();
        var request = new CreateTeacherAssignmentRequest { TeacherId = studentUserId, ClassSubjectId = classSubjectId };

        await Assert.ThrowsAsync<ConflictException>(() => service.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_WhenAlreadyAssigned_ThrowsConflictException()
    {
        var teacherId = Guid.NewGuid();
        var classSubjectId = Guid.NewGuid();

        _builder.Users.Setup(r => r.GetByIdAsync(teacherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = teacherId, Role = UserRole.Teacher, FullName = "Ms. Smith" });
        _builder.ClassSubjects.Setup(r => r.GetByIdAsync(classSubjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ClassSubject
            {
                Id = classSubjectId,
                ClassCourse = new ClassCourse { Name = "Grade 10" },
                Subject = new Subject { Name = "Math", Code = "MATH101" }
            });
        _builder.TeacherAssignments.Setup(r => r.ExistsAsync(teacherId, classSubjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = CreateService();
        var request = new CreateTeacherAssignmentRequest { TeacherId = teacherId, ClassSubjectId = classSubjectId };

        await Assert.ThrowsAsync<ConflictException>(() => service.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_WithValidTeacher_CreatesAssignment()
    {
        var teacherId = Guid.NewGuid();
        var classSubjectId = Guid.NewGuid();

        _builder.Users.Setup(r => r.GetByIdAsync(teacherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = teacherId, Role = UserRole.Teacher, FullName = "Ms. Smith", Email = "smith@test.com" });
        _builder.ClassSubjects.Setup(r => r.GetByIdAsync(classSubjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ClassSubject
            {
                Id = classSubjectId,
                ClassCourse = new ClassCourse { Name = "Grade 10" },
                Subject = new Subject { Name = "Math", Code = "MATH101" }
            });
        _builder.TeacherAssignments.Setup(r => r.ExistsAsync(teacherId, classSubjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var service = CreateService();
        var request = new CreateTeacherAssignmentRequest { TeacherId = teacherId, ClassSubjectId = classSubjectId };

        var result = await service.CreateAsync(request);

        Assert.Equal("Ms. Smith", result.TeacherName);
        Assert.Equal("Grade 10", result.ClassCourseName);
    }
}