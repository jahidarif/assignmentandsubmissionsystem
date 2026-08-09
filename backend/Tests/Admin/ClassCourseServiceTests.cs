using AssignmentSubmissionSystem.Application.Common.Exceptions;
using AssignmentSubmissionSystem.Application.Features.Admin.ClassCourses;
using AssignmentSubmissionSystem.Application.Features.Admin.ClassCourses.Dtos;
using AssignmentSubmissionSystem.Application.Features.Admin.ClassCourses.Validators;
using AssignmentSubmissionSystem.Domain.Entities;
using Moq;
using Tests.Common;
using Xunit;

namespace Tests.Features.Admin;

public class ClassCourseServiceTests
{
    private readonly MockUnitOfWorkBuilder _builder = new();

    private ClassCourseService CreateService()
        => new(_builder.Build().Object, new CreateClassCourseRequestValidator(), new UpdateClassCourseRequestValidator());

    [Fact]
    public async Task UpdateAsync_WhenNotFound_ThrowsNotFoundException()
    {
        var id = Guid.NewGuid();
        _builder.ClassCourses.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ClassCourse?)null);

        var service = CreateService();
        var request = new UpdateClassCourseRequest { Name = "Grade 11" };

        await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateAsync(id, request));
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ReturnsCreatedClassCourse()
    {
        var service = CreateService();
        var request = new CreateClassCourseRequest { Name = "Grade 10", Section = "A" };

        var result = await service.CreateAsync(request);

        Assert.Equal("Grade 10", result.Name);
        Assert.Equal("A", result.Section);
        _builder.ClassCourses.Verify(r => r.AddAsync(It.IsAny<ClassCourse>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}