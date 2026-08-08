using AssignmentSubmissionSystem.Application.Features.Admin.ClassCourses.Dtos;
using FluentValidation;

namespace AssignmentSubmissionSystem.Application.Features.Admin.ClassCourses.Validators;

public class CreateClassCourseRequestValidator : AbstractValidator<CreateClassCourseRequest>
{
    public CreateClassCourseRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Section).MaximumLength(50);
    }
}

public class UpdateClassCourseRequestValidator : AbstractValidator<UpdateClassCourseRequest>
{
    public UpdateClassCourseRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Section).MaximumLength(50);
    }
}