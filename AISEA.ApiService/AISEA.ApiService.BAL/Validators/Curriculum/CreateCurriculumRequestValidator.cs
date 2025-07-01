using AISEA.ApiService.SHARED.DTOs.Requests.Curriculum;
using FluentValidation;

namespace AISEA.ApiService.BAL.Validators.Curriculum
{
    public class CreateCurriculumRequestValidator : AbstractValidator<CreateCurriculumRequest>
    {
        public CreateCurriculumRequestValidator()
        {
            RuleFor(x => x.ProgramId)
                .GreaterThan(0).WithMessage("Program ID must be greater than 0.");

            RuleFor(x => x.CurriculumCode)
                .NotEmpty().WithMessage("Curriculum code is required.")
                .MaximumLength(50).WithMessage("Curriculum code must be less than 50 characters.")
                .Matches(@"^[A-Za-z0-9]+$").WithMessage("Curriculum code must contain only letters and numbers.");

            RuleFor(x => x.CurriculumName)
                .NotEmpty().WithMessage("Curriculum name is required.")
                .MaximumLength(255).WithMessage("Curriculum name must be less than 255 characters.");

            RuleFor(x => x.EffectiveDate)
                .NotEmpty().WithMessage("Effective date is required.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description must be less than 1000 characters.")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleForEach(x => x.Subjects)
                .SetValidator(new CurriculumSubjectRequestValidator())
                .When(x => x.Subjects != null);
        }
    }

    public class CurriculumSubjectRequestValidator : AbstractValidator<CurriculumSubjectRequest>
    {
        public CurriculumSubjectRequestValidator()
        {
            RuleFor(x => x.SubjectId)
                .GreaterThan(0).WithMessage("Subject ID must be greater than 0.");

            RuleFor(x => x.SemesterNumber)
                .InclusiveBetween(1, 8).WithMessage("Semester number must be between 1 and 8.");
        }
    }
}