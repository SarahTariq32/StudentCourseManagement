//using FluentValidation;
//using StudentCourseManagement.Application.DTOs;

//namespace StudentCourseManagement.Application.Validators;

//public class RegisterDtoValidator : AbstractValidator<RegisterDto>
//{
//    public RegisterDtoValidator()
//    {
//        RuleFor(x => x.Username)
//            .NotEmpty().WithMessage("Username is required.")
//            .Length(3, 50).WithMessage("Username must be between 3 and 50 characters.");

//        RuleFor(x => x.Password)
//            .NotEmpty().WithMessage("Password is required.")
//            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

//        RuleFor(x => x.Role)
//            .NotEmpty().WithMessage("Role is required.")
//            .Must(role => role.Equals("Admin", StringComparison.OrdinalIgnoreCase) || role.Equals("Student", StringComparison.OrdinalIgnoreCase))
//            .WithMessage("Role must be either 'Admin' or 'Student'.");
//    }
//}


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