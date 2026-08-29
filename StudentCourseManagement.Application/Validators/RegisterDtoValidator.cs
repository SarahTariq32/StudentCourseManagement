using FluentValidation;
using StudentCourseManagement.Application.DTOs;
using StudentCourseManagement.Domain.Enums;

namespace StudentCourseManagement.Application.Validators;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MaximumLength(100).WithMessage("Username must not exceed 100 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("Invalid role selection.");
    }
}