using AssignmentSubmissionSystem.Application.Common.Exceptions;
using AssignmentSubmissionSystem.Application.Features.Admin.Subjects;
using AssignmentSubmissionSystem.Application.Features.Admin.Subjects.Dtos;
using AssignmentSubmissionSystem.Application.Features.Admin.Subjects.Validators;
using AssignmentSubmissionSystem.Domain.Entities;
using Moq;
using Tests.Common;
using Xunit;

namespace Tests.Features.Admin;

public class SubjectServiceTests
{
    private readonly MockUnitOfWorkBuilder _builder = new();

    private SubjectService CreateService()
        => new(_builder.Build().Object, new CreateSubjectRequestValidator(), new UpdateSubjectRequestValidator());

    [Fact]
    public async Task DeleteAsync_WhenNotFound_ThrowsNotFoundException()
    {
        var id = Guid.NewGuid();
        _builder.Subjects.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Subject?)null);

        var service = CreateService();

        await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteAsync(id));
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ReturnsCreatedSubject()
    {
        var service = CreateService();
        var request = new CreateSubjectRequest { Name = "Physics", Code = "PHY101" };

        var result = await service.CreateAsync(request);

        Assert.Equal("Physics", result.Name);
        Assert.Equal("PHY101", result.Code);
    }
}