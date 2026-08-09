using AssignmentSubmissionSystem.Application.Common.Exceptions;
using AssignmentSubmissionSystem.Application.Common.Interfaces;
using AssignmentSubmissionSystem.Application.Common.Models;
using AssignmentSubmissionSystem.Application.Features.Auth;
using AssignmentSubmissionSystem.Application.Features.Auth.Dtos;
using AssignmentSubmissionSystem.Application.Features.Auth.Validators;
using AssignmentSubmissionSystem.Domain.Entities;
using AssignmentSubmissionSystem.Domain.Enums;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Tests.Common;
using Xunit;

namespace Tests.Features.Auth;

public class AuthServiceTests
{
    private readonly MockUnitOfWorkBuilder _builder = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGenerator = new();

    private AuthService CreateService()
    {
        var jwtSettings = Options.Create(new JwtSettings
        {
            Secret = "test-secret",
            Issuer = "test-issuer",
            Audience = "test-audience",
            AccessTokenExpirationMinutes = 15,
            RefreshTokenExpirationDays = 7
        });

        return new AuthService(
            _builder.Build().Object,
            _passwordHasher.Object,
            _jwtTokenGenerator.Object,
            jwtSettings,
            new RegisterRequestValidator(),
            new LoginRequestValidator(),
            new RefreshTokenRequestValidator(),
            NullLogger<AuthService>.Instance);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ThrowsConflictException()
    {
        _builder.Users.Setup(r => r.EmailExistsAsync("student@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = CreateService();

        var request = new RegisterRequest
        {
            FullName = "Test Student",
            Email = "student@test.com",
            Password = "Password1",
            Role = UserRole.Student
        };

        await Assert.ThrowsAsync<ConflictException>(() => service.RegisterAsync(request));
    }

    [Fact]
    public async Task RegisterAsync_WithValidData_CreatesUserAndReturnsResponse()
    {
        _builder.Users.Setup(r => r.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _passwordHasher.Setup(h => h.Hash(It.IsAny<string>())).Returns("hashed-password");

        var service = CreateService();

        var request = new RegisterRequest
        {
            FullName = "Test Teacher",
            Email = "teacher@test.com",
            Password = "Password1",
            Role = UserRole.Teacher
        };

        var result = await service.RegisterAsync(request);

        Assert.Equal("teacher@test.com", result.Email);
        Assert.Equal("Teacher", result.Role);
        _builder.Users.Verify(r => r.AddAsync(It.Is<User>(u => u.PasswordHash == "hashed-password"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ThrowsUnauthorizedException()
    {
        var user = new User
        {
            Email = "student@test.com",
            PasswordHash = "hashed",
            Role = UserRole.Student,
            IsActive = true
        };

        _builder.Users.Setup(r => r.GetByEmailAsync("student@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasher.Setup(h => h.Verify("wrong-password", "hashed")).Returns(false);

        var service = CreateService();

        var request = new LoginRequest { Email = "student@test.com", Password = "wrong-password" };

        await Assert.ThrowsAsync<UnauthorizedException>(() => service.LoginAsync(request));
    }

    [Fact]
    public async Task LoginAsync_WithDeactivatedUser_ThrowsUnauthorizedException()
    {
        var user = new User
        {
            Email = "student@test.com",
            PasswordHash = "hashed",
            Role = UserRole.Student,
            IsActive = false
        };

        _builder.Users.Setup(r => r.GetByEmailAsync("student@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasher.Setup(h => h.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

        var service = CreateService();

        var request = new LoginRequest { Email = "student@test.com", Password = "Password1" };

        await Assert.ThrowsAsync<UnauthorizedException>(() => service.LoginAsync(request));
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsTokens()
    {
        var user = new User
        {
            Email = "student@test.com",
            PasswordHash = "hashed",
            Role = UserRole.Student,
            IsActive = true
        };

        _builder.Users.Setup(r => r.GetByEmailAsync("student@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasher.Setup(h => h.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
        _jwtTokenGenerator.Setup(g => g.GenerateAccessToken(user)).Returns("access-token");
        _jwtTokenGenerator.Setup(g => g.GenerateRefreshToken()).Returns("refresh-token");

        var service = CreateService();

        var request = new LoginRequest { Email = "student@test.com", Password = "Password1" };

        var result = await service.LoginAsync(request);

        Assert.Equal("access-token", result.AccessToken);
        Assert.Equal("refresh-token", result.RefreshToken);
        _builder.RefreshTokens.Verify(r => r.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}