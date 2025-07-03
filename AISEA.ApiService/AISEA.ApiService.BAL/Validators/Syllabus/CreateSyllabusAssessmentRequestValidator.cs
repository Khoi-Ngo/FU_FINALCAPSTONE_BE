using AISEA.ApiService.SHARED.DTOs.Requests.Syllabus;
using FluentValidation;

namespace AISEA.ApiService.BAL.Validators.Syllabus
{
    public class CreateSyllabusAssessmentRequestValidator : AbstractValidator<CreateSyllabusAssessmentRequest>
    {
        public CreateSyllabusAssessmentRequestValidator()
        {
            RuleFor(x => x.SyllabusId)
                .GreaterThan(0).WithMessage("Syllabus ID must be greater than 0.");

            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("Category is required.")
                .MaximumLength(100).WithMessage("Category must be less than 100 characters.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.");

            RuleFor(x => x.Weight)
                .GreaterThan(0).WithMessage("Weight must be greater than 0.")
                .LessThanOrEqualTo(100).WithMessage("Weight must be less than or equal to 100.");

            RuleFor(x => x.Duration)
                .GreaterThan(0).WithMessage("Duration must be greater than 0.")
                .When(x => x.Duration.HasValue);

            RuleFor(x => x.QuestionType)
                .MaximumLength(255).WithMessage("Question type must be less than 255 characters.")
                .When(x => !string.IsNullOrEmpty(x.QuestionType));
        }
    }
}