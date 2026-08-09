using AssignmentSubmissionSystem.Application.Common.Exceptions;
using AssignmentSubmissionSystem.Application.Features.Admin.ClassSubjects;
using AssignmentSubmissionSystem.Application.Features.Admin.ClassSubjects.Dtos;
using AssignmentSubmissionSystem.Application.Features.Admin.ClassSubjects.Validators;
using AssignmentSubmissionSystem.Domain.Entities;
using Moq;
using Tests.Common;
using Xunit;

namespace Tests.Features.Admin;

public class ClassSubjectServiceTests
{
    private readonly MockUnitOfWorkBuilder _builder = new();

    private ClassSubjectService CreateService()
        => new(_builder.Build().Object, new CreateClassSubjectRequestValidator());

    [Fact]
    public async Task CreateAsync_WhenClassCourseNotFound_ThrowsNotFoundException()
    {
        var classCourseId = Guid.NewGuid();
        var subjectId = Guid.NewGuid();

        _builder.ClassCourses.Setup(r => r.GetByIdAsync(classCourseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ClassCourse?)null);

        var service = CreateService();
        var request = new CreateClassSubjectRequest { ClassCourseId = classCourseId, SubjectId = subjectId };

        await Assert.ThrowsAsync<NotFoundException>(() => service.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_WhenLinkAlreadyExists_ThrowsConflictException()
    {
        var classCourseId = Guid.NewGuid();
        var subjectId = Guid.NewGuid();

        _builder.ClassCourses.Setup(r => r.GetByIdAsync(classCourseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ClassCourse { Id = classCourseId, Name = "Grade 10" });
        _builder.Subjects.Setup(r => r.GetByIdAsync(subjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Subject { Id = subjectId, Name = "Math", Code = "MATH101" });
        _builder.ClassSubjects.Setup(r => r.ExistsAsync(classCourseId, subjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = CreateService();
        var request = new CreateClassSubjectRequest { ClassCourseId = classCourseId, SubjectId = subjectId };

        await Assert.ThrowsAsync<ConflictException>(() => service.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_WithValidData_CreatesLink()
    {
        var classCourseId = Guid.NewGuid();
        var subjectId = Guid.NewGuid();

        _builder.ClassCourses.Setup(r => r.GetByIdAsync(classCourseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ClassCourse { Id = classCourseId, Name = "Grade 10" });
        _builder.Subjects.Setup(r => r.GetByIdAsync(subjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Subject { Id = subjectId, Name = "Math", Code = "MATH101" });
        _builder.ClassSubjects.Setup(r => r.ExistsAsync(classCourseId, subjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var service = CreateService();
        var request = new CreateClassSubjectRequest { ClassCourseId = classCourseId, SubjectId = subjectId };

        var result = await service.CreateAsync(request);

        Assert.Equal("Grade 10", result.ClassCourseName);
        Assert.Equal("Math", result.SubjectName);
        _builder.ClassSubjects.Verify(r => r.AddAsync(It.IsAny<ClassSubject>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}