using AISEA.ApiService.SHARED.DTOs.Requests.SystemProfile;
using FluentValidation;

namespace AISEA.ApiService.BAL.Validators.SystemProfile
{
    public class CreateStudentProfileRequestValidator : AbstractValidator<CreateStudentProfileRequest>
    {
        public CreateStudentProfileRequestValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("UserId must be greater than 0.");

            RuleFor(x => x.EnrolledAt)
                .NotEmpty().WithMessage("EnrolledAt is required.");

            RuleFor(x => x.CareerGoal)
                .MaximumLength(1000).WithMessage("CareerGoal must be less than 1000 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.CareerGoal));

            RuleFor(x => x.ProgramId)
                .GreaterThan(0).WithMessage("ProgramId must be greater than 0.");
            RuleFor(x => x.CurriculumCode)
                .NotEmpty().WithMessage("CurriculumCode is required.");
        }
    }
}