using AssignmentSubmissionSystem.Application.Features.Admin.Users;
using AssignmentSubmissionSystem.Application.Features.Admin.Users.Dtos;
using AssignmentSubmissionSystem.Application.Features.Admin.Users.Validators;
using AssignmentSubmissionSystem.Domain.Entities;
using AssignmentSubmissionSystem.Domain.Enums;
using Moq;
using Tests.Common;
using Xunit;

namespace Tests.Features.Admin;

public class UserManagementServiceTests
{
    private readonly MockUnitOfWorkBuilder _builder = new();

    private UserManagementService CreateService()
        => new(_builder.Build().Object, new UpdateUserRequestValidator());

    [Fact]
    public async Task DeactivateUserAsync_SetsIsActiveFalse()
    {
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, IsActive = true, Role = UserRole.Teacher };

        _builder.Users.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var service = CreateService();
        await service.DeactivateUserAsync(userId);

        Assert.False(user.IsActive);
        _builder.Users.Verify(r => r.Update(It.Is<User>(u => u.IsActive == false)), Times.Once);
    }

    [Fact]
    public async Task ReactivateUserAsync_SetsIsActiveTrue()
    {
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, IsActive = false, Role = UserRole.Student };

        _builder.Users.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var service = CreateService();
        await service.ReactivateUserAsync(userId);

        Assert.True(user.IsActive);
    }

    [Fact]
    public async Task GetUsersAsync_ReturnsPagedResult()
    {
        var users = new List<User>
        {
            new() { FullName = "Alice", Email = "alice@test.com", Role = UserRole.Student, IsActive = true },
            new() { FullName = "Bob", Email = "bob@test.com", Role = UserRole.Teacher, IsActive = true }
        };

        _builder.Users
            .Setup(r => r.GetPagedAsync(1, 10, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((users, 2));

        var service = CreateService();
        var result = await service.GetUsersAsync(1, null, null);

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
    }

    [Fact]
    public async Task GetTeachersLookupAsync_ReturnsOnlyActiveTeachers()
    {
        var teachers = new List<User>
        {
            new() { FullName = "Ms. Smith", Email = "smith@test.com", Role = UserRole.Teacher, IsActive = true }
        };

        _builder.Users
            .Setup(r => r.GetActiveByRoleAsync(UserRole.Teacher, It.IsAny<CancellationToken>()))
            .ReturnsAsync(teachers);

        var service = CreateService();
        var result = await service.GetTeachersLookupAsync();

        Assert.Single(result);
        Assert.Equal("Ms. Smith", result[0].FullName);
    }
}