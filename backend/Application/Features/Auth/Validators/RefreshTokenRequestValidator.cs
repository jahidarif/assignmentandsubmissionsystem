using AssignmentSubmissionSystem.Application.Features.Auth.Dtos;
using FluentValidation;

namespace AssignmentSubmissionSystem.Application.Features.Auth.Validators;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.");
    }
}