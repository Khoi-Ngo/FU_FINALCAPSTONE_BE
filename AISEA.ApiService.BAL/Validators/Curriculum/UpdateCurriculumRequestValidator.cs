using AISEA.ApiService.SHARED.DTOs.Requests.Curriculum;
using FluentValidation;

namespace AISEA.ApiService.BAL.Validators.Curriculum
{
    public class UpdateCurriculumRequestValidator : AbstractValidator<UpdateCurriculumRequest>
    {
        public UpdateCurriculumRequestValidator()
        {
            RuleFor(x => x.ProgramId)
                .GreaterThan(0).WithMessage("Program ID must be greater than 0.");

            RuleFor(x => x.CurriculumCode)
                .NotEmpty().WithMessage("Curriculum code is required.")
                .MaximumLength(50).WithMessage("Curriculum code must be less than 50 characters.")
                .Matches(@"^[A-Z0-9_\-]+$").WithMessage("Curriculum code must contain only uppercase letters, numbers, underscores, and hyphens.");

            RuleFor(x => x.CurriculumName)
                .NotEmpty().WithMessage("Curriculum name is required.")
                .MaximumLength(255).WithMessage("Curriculum name must be less than 255 characters.");

            RuleFor(x => x.EffectiveDate)
                .NotEmpty().WithMessage("Effective date is required.");
        }
    }
}