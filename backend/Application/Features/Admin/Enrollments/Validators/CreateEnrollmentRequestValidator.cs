using AssignmentSubmissionSystem.Application.Features.Admin.Enrollments.Dtos;
using FluentValidation;

namespace AssignmentSubmissionSystem.Application.Features.Admin.Enrollments.Validators;

public class CreateEnrollmentRequestValidator : AbstractValidator<CreateEnrollmentRequest>
{
    public CreateEnrollmentRequestValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.ClassCourseId).NotEmpty();
    }
}