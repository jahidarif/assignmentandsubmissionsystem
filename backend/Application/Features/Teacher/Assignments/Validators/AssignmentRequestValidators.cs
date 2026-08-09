using AssignmentSubmissionSystem.Application.Features.Teacher.Assignments.Dtos;
using AssignmentSubmissionSystem.Domain.Enums;
using FluentValidation;

namespace AssignmentSubmissionSystem.Application.Features.Teacher.Assignments.Validators;

public class CreateAssignmentRequestValidator : AbstractValidator<CreateAssignmentRequest>
{
    public CreateAssignmentRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Deadline).GreaterThan(DateTime.UtcNow).WithMessage("Deadline must be in the future.");
        RuleFor(x => x.MaxMarks).GreaterThan(0);
        RuleFor(x => x.ClassSubjectId).NotEmpty();
        RuleFor(x => x.Status).Must(s => s is AssignmentStatus.Draft or AssignmentStatus.Published)
            .WithMessage("Status must be Draft or Published.");
    }
}

public class UpdateAssignmentRequestValidator : AbstractValidator<UpdateAssignmentRequest>
{
    public UpdateAssignmentRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Deadline).GreaterThan(DateTime.UtcNow).WithMessage("Deadline must be in the future.");
        RuleFor(x => x.MaxMarks).GreaterThan(0);
    }
}