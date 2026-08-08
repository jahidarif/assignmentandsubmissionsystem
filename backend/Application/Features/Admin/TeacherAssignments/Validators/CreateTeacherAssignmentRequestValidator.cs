using AssignmentSubmissionSystem.Application.Features.Admin.TeacherAssignments.Dtos;
using FluentValidation;

namespace AssignmentSubmissionSystem.Application.Features.Admin.TeacherAssignments.Validators;

public class CreateTeacherAssignmentRequestValidator : AbstractValidator<CreateTeacherAssignmentRequest>
{
    public CreateTeacherAssignmentRequestValidator()
    {
        RuleFor(x => x.TeacherId).NotEmpty();
        RuleFor(x => x.ClassSubjectId).NotEmpty();
    }
}