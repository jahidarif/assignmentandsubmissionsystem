using AssignmentSubmissionSystem.Application.Features.Teacher.Submissions.Dtos;
using FluentValidation;

namespace AssignmentSubmissionSystem.Application.Features.Teacher.Submissions.Validators;

public class GradeSubmissionRequestValidator : AbstractValidator<GradeSubmissionRequest>
{
    public GradeSubmissionRequestValidator()
    {
        RuleFor(x => x.Marks).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Feedback).MaximumLength(2000);
    }
}

public class UpdateSubmissionStatusRequestValidator : AbstractValidator<UpdateSubmissionStatusRequest>
{
    public UpdateSubmissionStatusRequestValidator()
    {
        RuleFor(x => x.Status).IsInEnum();
    }
}