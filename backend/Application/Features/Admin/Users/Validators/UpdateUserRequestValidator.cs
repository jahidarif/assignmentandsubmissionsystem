using AssignmentSubmissionSystem.Application.Features.Admin.Users.Dtos;
using FluentValidation;

namespace AssignmentSubmissionSystem.Application.Features.Admin.Users.Validators;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
    }
}