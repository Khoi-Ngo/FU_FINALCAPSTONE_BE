using AISEA.ApiService.SHARED.DTOs.Requests.Syllabus;
using FluentValidation;

namespace AISEA.ApiService.BAL.Validators.Syllabus
{
    public class CreateSyllabusRequestValidator : AbstractValidator<CreateSyllabusRequest>
    {
        public CreateSyllabusRequestValidator()
        {
            RuleFor(x => x.SubjectVersionId)
                .GreaterThan(0).WithMessage("Subject Version ID must be greater than 0.");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required.")
                .MinimumLength(10).WithMessage("Content must be at least 10 characters.");
        }
    }
}