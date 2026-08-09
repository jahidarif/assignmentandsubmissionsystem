using AssignmentSubmissionSystem.Application.Common.Exceptions;
using AssignmentSubmissionSystem.Application.Features.Admin.Enrollments;
using AssignmentSubmissionSystem.Application.Features.Admin.Enrollments.Dtos;
using AssignmentSubmissionSystem.Application.Features.Admin.Enrollments.Validators;
using AssignmentSubmissionSystem.Domain.Entities;
using AssignmentSubmissionSystem.Domain.Enums;
using Moq;
using Tests.Common;
using Xunit;

namespace Tests.Features.Admin;

public class EnrollmentServiceTests
{
    private readonly MockUnitOfWorkBuilder _builder = new();

    private EnrollmentService CreateService()
        => new(_builder.Build().Object, new CreateEnrollmentRequestValidator());

    [Fact]
    public async Task CreateAsync_WhenUserIsNotAStudent_ThrowsConflictException()
    {
        var teacherUserId = Guid.NewGuid();
        var classCourseId = Guid.NewGuid();

        _builder.Users.Setup(r => r.GetByIdAsync(teacherUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = teacherUserId, Role = UserRole.Teacher, FullName = "Not A Student" });

        var service = CreateService();
        var request = new CreateEnrollmentRequest { StudentId = teacherUserId, ClassCourseId = classCourseId };

        await Assert.ThrowsAsync<ConflictException>(() => service.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_WhenAlreadyEnrolled_ThrowsConflictException()
    {
        var studentId = Guid.NewGuid();
        var classCourseId = Guid.NewGuid();

        _builder.Users.Setup(r => r.GetByIdAsync(studentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = studentId, Role = UserRole.Student, FullName = "John Student" });
        _builder.ClassCourses.Setup(r => r.GetByIdAsync(classCourseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ClassCourse { Id = classCourseId, Name = "Grade 10" });
        _builder.Enrollments.Setup(r => r.ExistsAsync(studentId, classCourseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = CreateService();
        var request = new CreateEnrollmentRequest { StudentId = studentId, ClassCourseId = classCourseId };

        await Assert.ThrowsAsync<ConflictException>(() => service.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_WithValidStudent_CreatesEnrollment()
    {
        var studentId = Guid.NewGuid();
        var classCourseId = Guid.NewGuid();

        _builder.Users.Setup(r => r.GetByIdAsync(studentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = studentId, Role = UserRole.Student, FullName = "John Student", Email = "john@test.com" });
        _builder.ClassCourses.Setup(r => r.GetByIdAsync(classCourseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ClassCourse { Id = classCourseId, Name = "Grade 10" });
        _builder.Enrollments.Setup(r => r.ExistsAsync(studentId, classCourseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var service = CreateService();
        var request = new CreateEnrollmentRequest { StudentId = studentId, ClassCourseId = classCourseId };

        var result = await service.CreateAsync(request);

        Assert.Equal("John Student", result.StudentName);
        Assert.Equal("Grade 10", result.ClassCourseName);
    }
}