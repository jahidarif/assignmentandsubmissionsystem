using AssignmentSubmissionSystem.Application.Common.Exceptions;
using AssignmentSubmissionSystem.Application.Common.Interfaces;
using AssignmentSubmissionSystem.Application.Common.Models;
using AssignmentSubmissionSystem.Application.Features.Admin.ClassSubjects.Dtos;
using AssignmentSubmissionSystem.Domain.Entities;
using FluentValidation;
using ValidationException = AssignmentSubmissionSystem.Application.Common.Exceptions.ValidationException;

namespace AssignmentSubmissionSystem.Application.Features.Admin.ClassSubjects;

public class ClassSubjectService : IClassSubjectService
{
    private const int PageSize = 10;

    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateClassSubjectRequest> _createValidator;

    public ClassSubjectService(IUnitOfWork unitOfWork, IValidator<CreateClassSubjectRequest> createValidator)
    {
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
    }

    public async Task<PagedResult<ClassSubjectDto>> GetClassSubjectsAsync(int page, Guid? classCourseId, CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;

        var (items, totalCount) = await _unitOfWork.ClassSubjects.GetPagedAsync(page, PageSize, classCourseId, cancellationToken);

        return new PagedResult<ClassSubjectDto>
        {
            Items = items.Select(MapToDto).ToList(),
            Page = page,
            PageSize = PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<ClassSubjectDto> CreateAsync(CreateClassSubjectRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var classCourse = await _unitOfWork.ClassCourses.GetByIdAsync(request.ClassCourseId, cancellationToken)
            ?? throw new NotFoundException("Class course not found.");

        var subject = await _unitOfWork.Subjects.GetByIdAsync(request.SubjectId, cancellationToken)
            ?? throw new NotFoundException("Subject not found.");

        var alreadyExists = await _unitOfWork.ClassSubjects.ExistsAsync(request.ClassCourseId, request.SubjectId, cancellationToken);
        if (alreadyExists)
        {
            throw new ConflictException("This subject is already offered in this class.");
        }

        var classSubject = new ClassSubject
        {
            ClassCourseId = request.ClassCourseId,
            SubjectId = request.SubjectId
        };

        await _unitOfWork.ClassSubjects.AddAsync(classSubject, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ClassSubjectDto
        {
            Id = classSubject.Id,
            ClassCourseId = classCourse.Id,
            ClassCourseName = classCourse.Name,
            SubjectId = subject.Id,
            SubjectName = subject.Name,
            SubjectCode = subject.Code
        };
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var classSubject = await _unitOfWork.ClassSubjects.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Class-subject link not found.");

        _unitOfWork.ClassSubjects.Remove(classSubject);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<ClassSubjectDto>> GetAllLookupAsync(CancellationToken cancellationToken = default)
    {
        var items = await _unitOfWork.ClassSubjects.GetAllWithDetailsAsync(cancellationToken);
        return items.Select(MapToDto).ToList();
    }

    private static ClassSubjectDto MapToDto(ClassSubject cs) => new()
    {
        Id = cs.Id,
        ClassCourseId = cs.ClassCourseId,
        ClassCourseName = cs.ClassCourse.Name,
        SubjectId = cs.SubjectId,
        SubjectName = cs.Subject.Name,
        SubjectCode = cs.Subject.Code
    };
}