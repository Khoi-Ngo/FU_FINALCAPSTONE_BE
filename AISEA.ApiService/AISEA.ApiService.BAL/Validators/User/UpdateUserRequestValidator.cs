using System;
using AISEA.ApiService.SHARED.DTOs.Requests.User;
using AISEA.ApiService.SHARED.Const.Enums;
using FluentValidation;

namespace AISEA.ApiService.BAL.Validators.User
{
    public class UpdateStudentRequestValidator : AbstractValidator<UpdateStudentRequest>
    {
        public UpdateStudentRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.")
                .MinimumLength(4).WithMessage("Username must be at least 4 characters long.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.");

            RuleFor(x => x.DateOfBirth)
                .NotNull().WithMessage("Date of birth is required.")
                .LessThan(DateTimeOffset.Now).WithMessage("Date of birth must be in the past.");

            RuleFor(x => x.RoleId)
                .GreaterThan(0).WithMessage("RoleId must be greater than 0.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid user status.");

            RuleFor(x => x.StudentDataUpdateRequest)
                .NotNull().WithMessage("Student data is required.")
                .SetValidator(new StudentDataUpdateRequestValidator());
        }
    }

    public class StudentDataUpdateRequestValidator : AbstractValidator<StudentDataUpdateRequest>
    {
        public StudentDataUpdateRequestValidator()
        {
            RuleFor(x => x.EnrolledAt)
                .LessThanOrEqualTo(DateTimeOffset.Now).WithMessage("EnrolledAt must be in the past or now.");

            RuleFor(x => x.CareerGoal)
                .MaximumLength(200).WithMessage("Career goal must be at most 200 characters.")
                .When(x => !string.IsNullOrEmpty(x.CareerGoal));
        }
    }

    public class UpdateStaffRequestValidator : AbstractValidator<UpdateStaffRequest>
    {
        public UpdateStaffRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.")
                .MinimumLength(4).WithMessage("Username must be at least 4 characters long.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.");

            RuleFor(x => x.DateOfBirth)
                .NotNull().WithMessage("Date of birth is required.")
                .LessThan(DateTimeOffset.Now).WithMessage("Date of birth must be in the past.");

            RuleFor(x => x.RoleId)
                .GreaterThan(0).WithMessage("RoleId must be greater than 0.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid user status.");

            RuleFor(x => x.StaffDataUpdateRequest)
                .NotNull().WithMessage("Staff data is required.")
                .SetValidator(new StaffDataUpdateRequestValidator());
        }
    }

    public class StaffDataUpdateRequestValidator : AbstractValidator<StaffDataUpdateRequest>
    {
        public StaffDataUpdateRequestValidator()
        {
            RuleFor(x => x.Campus)
                .NotEmpty().WithMessage("Campus is required.")
                .MaximumLength(255).WithMessage("Campus must be at most 255 characters.");

            RuleFor(x => x.Department)
                .NotEmpty().WithMessage("Department is required.")
                .MaximumLength(255).WithMessage("Department must be at most 255 characters.");

            RuleFor(x => x.Position)
                .NotEmpty().WithMessage("Position is required.")
                .MaximumLength(255).WithMessage("Position must be at most 255 characters.");

            RuleFor(x => x.StartWorkAt)
                .LessThanOrEqualTo(DateTimeOffset.Now).WithMessage("StartWorkAt must be in the past or now.")
                .When(x => x.StartWorkAt.HasValue);

            RuleFor(x => x.EndWorkAt)
                .GreaterThan(x => x.StartWorkAt)
                .WithMessage("EndWorkAt must be after StartWorkAt.")
                .When(x => x.EndWorkAt.HasValue && x.StartWorkAt.HasValue);
        }
    }
}