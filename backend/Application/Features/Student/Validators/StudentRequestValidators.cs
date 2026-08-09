using AssignmentSubmissionSystem.Application.Features.Student.Dtos;
using FluentValidation;

namespace AssignmentSubmissionSystem.Application.Features.Student.Validators;

public class SubmitAssignmentRequestValidator : AbstractValidator<SubmitAssignmentRequest>
{
    public SubmitAssignmentRequestValidator()
    {
        RuleFor(x => x.AnswerText).NotEmpty().MaximumLength(10000);
        RuleFor(x => x.AttachmentUrl).MaximumLength(2000);
    }
}

public class UpdateSubmissionRequestValidator : AbstractValidator<UpdateSubmissionRequest>
{
    public UpdateSubmissionRequestValidator()
    {
        RuleFor(x => x.AnswerText).NotEmpty().MaximumLength(10000);
        RuleFor(x => x.AttachmentUrl).MaximumLength(2000);
    }
}