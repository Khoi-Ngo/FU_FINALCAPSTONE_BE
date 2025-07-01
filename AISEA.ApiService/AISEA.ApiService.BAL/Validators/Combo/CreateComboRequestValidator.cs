using AISEA.ApiService.SHARED.DTOs.Requests.Combo;
using FluentValidation;

namespace AISEA.ApiService.BAL.Validators.Combo
{
    public class CreateComboRequestValidator : AbstractValidator<CreateComboRequest>
    {
        public CreateComboRequestValidator()
        {
            RuleFor(x => x.ComboName)
                .NotEmpty().WithMessage("Combo name is required.")
                .MaximumLength(255).WithMessage("Combo name must be less than 255 characters.");

            RuleFor(x => x.ComboDescription)
                .MaximumLength(1000).WithMessage("Combo description must be less than 1000 characters.")
                .When(x => !string.IsNullOrEmpty(x.ComboDescription));

            RuleFor(x => x.SemesterNumber)
                .InclusiveBetween(1, 8).WithMessage("Semester number must be between 1 and 8.");

            RuleFor(x => x.ProgramId)
                .GreaterThan(0).WithMessage("Program ID must be greater than 0.");

            RuleFor(x => x.DifficultyLevel)
                .NotEmpty().WithMessage("Difficulty level is required.")
                .Must(x => new[] { "Easy", "Medium", "Hard" }.Contains(x))
                .WithMessage("Difficulty level must be Easy, Medium, or Hard.");

            RuleFor(x => x.MaxStudents)
                .GreaterThan(0).WithMessage("Maximum students must be greater than 0.")
                .LessThanOrEqualTo(100).WithMessage("Maximum students cannot exceed 100.");

            RuleFor(x => x.SubjectIds)
                .NotEmpty().WithMessage("At least one subject must be selected.")
                .Must(x => x.Count <= 10).WithMessage("Cannot have more than 10 subjects in a combo.");

            RuleForEach(x => x.SubjectIds)
                .GreaterThan(0).WithMessage("Subject ID must be greater than 0.");

            RuleForEach(x => x.Prerequisites)
                .SetValidator(new ComboPrerequisiteRequestValidator())
                .When(x => x.Prerequisites != null);
        }
    }

    public class ComboPrerequisiteRequestValidator : AbstractValidator<ComboPrerequisiteRequest>
    {
        public ComboPrerequisiteRequestValidator()
        {
            RuleFor(x => x.SubjectId)
                .GreaterThan(0).WithMessage("Subject ID must be greater than 0.");
        }
    }
}