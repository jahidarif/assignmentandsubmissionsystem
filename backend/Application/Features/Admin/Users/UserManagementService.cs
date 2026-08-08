using AssignmentSubmissionSystem.Application.Common.Exceptions;
using AssignmentSubmissionSystem.Application.Common.Interfaces;
using AssignmentSubmissionSystem.Application.Common.Models;
using AssignmentSubmissionSystem.Application.Features.Admin.Users.Dtos;
using AssignmentSubmissionSystem.Domain.Entities;
using AssignmentSubmissionSystem.Domain.Enums;
using FluentValidation;
using ValidationException = AssignmentSubmissionSystem.Application.Common.Exceptions.ValidationException;

namespace AssignmentSubmissionSystem.Application.Features.Admin.Users;

public class UserManagementService : IUserManagementService
{
    private const int PageSize = 10;

    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateUserRequest> _updateValidator;

    public UserManagementService(IUnitOfWork unitOfWork, IValidator<UpdateUserRequest> updateValidator)
    {
        _unitOfWork = unitOfWork;
        _updateValidator = updateValidator;
    }

    public async Task<PagedResult<UserListItemDto>> GetUsersAsync(int page, UserRole? role, bool? isActive, CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;

        var (items, totalCount) = await _unitOfWork.Users.GetPagedAsync(page, PageSize, role, isActive, cancellationToken);

        return new PagedResult<UserListItemDto>
        {
            Items = items.Select(MapToDto).ToList(),
            Page = page,
            PageSize = PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<UserListItemDto> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        return MapToDto(user);
    }

    public async Task<UserListItemDto> UpdateUserAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        user.FullName = request.FullName.Trim();
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(user);
    }

    public async Task DeactivateUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        user.IsActive = false;
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ReactivateUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        user.IsActive = true;
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<UserListItemDto>> GetTeachersLookupAsync(CancellationToken cancellationToken = default)
    {
        var teachers = await _unitOfWork.Users.GetActiveByRoleAsync(UserRole.Teacher, cancellationToken);
        return teachers.Select(MapToDto).ToList();
    }

    public async Task<List<UserListItemDto>> GetStudentsLookupAsync(CancellationToken cancellationToken = default)
    {
        var students = await _unitOfWork.Users.GetActiveByRoleAsync(UserRole.Student, cancellationToken);
        return students.Select(MapToDto).ToList();
    }

    private static UserListItemDto MapToDto(User user) => new()
    {
        Id = user.Id,
        FullName = user.FullName,
        Email = user.Email,
        Role = user.Role.ToString(),
        IsActive = user.IsActive,
        CreatedAt = user.CreatedAt
    };
}