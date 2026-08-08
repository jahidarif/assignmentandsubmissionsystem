using AssignmentSubmissionSystem.Application.Common.Exceptions;
using AssignmentSubmissionSystem.Application.Common.Interfaces;
using AssignmentSubmissionSystem.Application.Common.Models;
using AssignmentSubmissionSystem.Application.Features.Admin.ClassCourses.Dtos;
using AssignmentSubmissionSystem.Domain.Entities;
using FluentValidation;
using ValidationException = AssignmentSubmissionSystem.Application.Common.Exceptions.ValidationException;

namespace AssignmentSubmissionSystem.Application.Features.Admin.ClassCourses;

public class ClassCourseService : IClassCourseService
{
    private const int PageSize = 10;

    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateClassCourseRequest> _createValidator;
    private readonly IValidator<UpdateClassCourseRequest> _updateValidator;

    public ClassCourseService(IUnitOfWork unitOfWork, IValidator<CreateClassCourseRequest> createValidator, IValidator<UpdateClassCourseRequest> updateValidator)
    {
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<PagedResult<ClassCourseDto>> GetClassCoursesAsync(int page, CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;

        var (items, totalCount) = await _unitOfWork.ClassCourses.GetPagedAsync(page, PageSize, cancellationToken);

        return new PagedResult<ClassCourseDto>
        {
            Items = items.Select(MapToDto).ToList(),
            Page = page,
            PageSize = PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<ClassCourseDto> CreateAsync(CreateClassCourseRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var classCourse = new ClassCourse
        {
            Name = request.Name.Trim(),
            Section = request.Section?.Trim()
        };

        await _unitOfWork.ClassCourses.AddAsync(classCourse, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(classCourse);
    }

    public async Task<ClassCourseDto> UpdateAsync(Guid id, UpdateClassCourseRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var classCourse = await _unitOfWork.ClassCourses.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Class course not found.");

        classCourse.Name = request.Name.Trim();
        classCourse.Section = request.Section?.Trim();

        _unitOfWork.ClassCourses.Update(classCourse);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(classCourse);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var classCourse = await _unitOfWork.ClassCourses.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Class course not found.");

        _unitOfWork.ClassCourses.Remove(classCourse);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<ClassCourseDto>> GetAllLookupAsync(CancellationToken cancellationToken = default)
    {
        var items = await _unitOfWork.ClassCourses.GetAllAsync(cancellationToken);
        return items.Select(MapToDto).ToList();
    }

    private static ClassCourseDto MapToDto(ClassCourse classCourse) => new()
    {
        Id = classCourse.Id,
        Name = classCourse.Name,
        Section = classCourse.Section
    };
}