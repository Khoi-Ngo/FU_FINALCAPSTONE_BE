using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISEA.ApiService.SHARED.DTOs.Requests.User;
using FluentValidation;

namespace AISEA.ApiService.BAL.Validators.User
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.")
                .MinimumLength(4).WithMessage("Username must be at least 4 characters long.")
                .MaximumLength(50).WithMessage("Username must be at most 50 characters long.")
                .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Username can only contain letters, numbers, and underscores.");


            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(100).WithMessage("First name must be at most 100 characters long.")
                .Matches(@"^[a-zA-Z\s]+$").WithMessage("First name can only contain letters and spaces.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(100).WithMessage("Last name must be at most 100 characters long.")
                .Matches(@"^[a-zA-Z\s]+$").WithMessage("Last name can only contain letters and spaces.");

            RuleFor(x => x.DateOfBirth)
                .NotNull().WithMessage("Date of birth is required.")
                .LessThan(DateTimeOffset.Now).WithMessage("Date of birth must be in the past.")
                .Must(dob => dob is null || dob.Value.AddYears(16) <= DateTimeOffset.Now)
                .WithMessage("User must be at least 16 years old.");


            RuleFor(x => x.RoleId)
                .GreaterThan(0).WithMessage("RoleId must be greater than 0.");


            RuleFor(x => new { x.StudentProfileData, x.StaffProfileData })
                    .Must(x =>
                        (x.StudentProfileData is null && x.StaffProfileData is null) ||
                        (x.StudentProfileData is not null && x.StaffProfileData is null) ||
                        (x.StudentProfileData is null && x.StaffProfileData is not null)
                    )
                    .WithMessage("You must provide either no profile, only StudentProfileData, or only StaffProfileData.");

            When(x => x.StudentProfileData is not null, () =>
        {
            RuleFor(x => x.StudentProfileData.EnrolledAt)
                .NotEmpty().WithMessage("EnrolledAt is required for student profile.");

            RuleFor(x => x.StudentProfileData.CareerGoal)
                .MaximumLength(1000).WithMessage("CareerGoal must be less than 1000 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.StudentProfileData.CareerGoal));
        });

            When(x => x.StaffProfileData is not null, () =>
            {
                RuleFor(x => x.StaffProfileData.Campus)
                    .NotEmpty().WithMessage("Campus is required for staff profile.")
                    .MaximumLength(255).WithMessage("Campus must be less than 255 characters.");

                RuleFor(x => x.StaffProfileData.Department)
                    .NotEmpty().WithMessage("Department is required for staff profile.")
                    .MaximumLength(255).WithMessage("Department must be less than 255 characters.");

                RuleFor(x => x.StaffProfileData.Position)
                    .NotEmpty().WithMessage("Position is required for staff profile.")
                    .MaximumLength(255).WithMessage("Position must be less than 255 characters.");

                RuleFor(x => x.StaffProfileData.EndWorkAt)
                    .GreaterThan(x => x.StaffProfileData.StartWorkAt)
                    .When(x => x.StaffProfileData.EndWorkAt.HasValue && x.StaffProfileData.StartWorkAt.HasValue)
                    .WithMessage("EndWorkAt must be after StartWorkAt.");
            });

        }
    }
}