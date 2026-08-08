using AssignmentSubmissionSystem.Application.Common.Exceptions;
using AssignmentSubmissionSystem.Application.Common.Interfaces;
using AssignmentSubmissionSystem.Application.Common.Models;
using AssignmentSubmissionSystem.Application.Features.Admin.Subjects.Dtos;
using AssignmentSubmissionSystem.Domain.Entities;
using FluentValidation;
using ValidationException = AssignmentSubmissionSystem.Application.Common.Exceptions.ValidationException;

namespace AssignmentSubmissionSystem.Application.Features.Admin.Subjects;

public class SubjectService : ISubjectService
{
    private const int PageSize = 10;

    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateSubjectRequest> _createValidator;
    private readonly IValidator<UpdateSubjectRequest> _updateValidator;

    public SubjectService(IUnitOfWork unitOfWork, IValidator<CreateSubjectRequest> createValidator, IValidator<UpdateSubjectRequest> updateValidator)
    {
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<PagedResult<SubjectDto>> GetSubjectsAsync(int page, CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;

        var (items, totalCount) = await _unitOfWork.Subjects.GetPagedAsync(page, PageSize, cancellationToken);

        return new PagedResult<SubjectDto>
        {
            Items = items.Select(MapToDto).ToList(),
            Page = page,
            PageSize = PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<SubjectDto> CreateAsync(CreateSubjectRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var subject = new Subject
        {
            Name = request.Name.Trim(),
            Code = request.Code.Trim()
        };

        await _unitOfWork.Subjects.AddAsync(subject, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(subject);
    }

    public async Task<SubjectDto> UpdateAsync(Guid id, UpdateSubjectRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var subject = await _unitOfWork.Subjects.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Subject not found.");

        subject.Name = request.Name.Trim();
        subject.Code = request.Code.Trim();

        _unitOfWork.Subjects.Update(subject);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(subject);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var subject = await _unitOfWork.Subjects.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Subject not found.");

        _unitOfWork.Subjects.Remove(subject);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<SubjectDto>> GetAllLookupAsync(CancellationToken cancellationToken = default)
    {
        var items = await _unitOfWork.Subjects.GetAllAsync(cancellationToken);
        return items.Select(MapToDto).ToList();
    }

    private static SubjectDto MapToDto(Subject subject) => new()
    {
        Id = subject.Id,
        Name = subject.Name,
        Code = subject.Code
    };
}