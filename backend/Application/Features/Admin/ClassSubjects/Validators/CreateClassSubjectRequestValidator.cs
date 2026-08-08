using AssignmentSubmissionSystem.Application.Features.Admin.ClassSubjects.Dtos;
using FluentValidation;

namespace AssignmentSubmissionSystem.Application.Features.Admin.ClassSubjects.Validators;

public class CreateClassSubjectRequestValidator : AbstractValidator<CreateClassSubjectRequest>
{
    public CreateClassSubjectRequestValidator()
    {
        RuleFor(x => x.ClassCourseId).NotEmpty().WithMessage("Class is required.");
        RuleFor(x => x.SubjectId).NotEmpty().WithMessage("Subject is required.");
    }
}