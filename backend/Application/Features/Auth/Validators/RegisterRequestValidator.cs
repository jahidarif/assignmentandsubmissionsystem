using AssignmentSubmissionSystem.Application.Features.Auth.Dtos;
using AssignmentSubmissionSystem.Domain.Enums;
using FluentValidation;

namespace AssignmentSubmissionSystem.Application.Features.Auth.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(150);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(256);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.");
        RuleFor(x => x.Role)
            .Must(role => role is UserRole.Teacher or UserRole.Student)
            .WithMessage("Role must be either Teacher or Student.");
    }
}