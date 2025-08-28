using AISEA.ApiService.SHARED.DTOs.Requests.Subject;
using FluentValidation;

namespace AISEA.ApiService.BAL.Validators.Subject
{
    public class CreateSubjectRequestValidator : AbstractValidator<CreateSubjectRequest>
    {
        public CreateSubjectRequestValidator()
        {
            RuleFor(x => x.SubjectCode)
                .NotEmpty().WithMessage("Subject code is required.")
                .MaximumLength(50).WithMessage("Subject code must be less than 50 characters.")
                .Matches(@"^[A-Za-z0-9]+$").WithMessage("Subject code must contain only letters (uppercase or lowercase) and numbers.");

            RuleFor(x => x.SubjectName)
                .NotEmpty().WithMessage("Subject name is required.")
                .MaximumLength(255).WithMessage("Subject name must be less than 255 characters.");

            RuleFor(x => x.Credits)
                .GreaterThanOrEqualTo(0).WithMessage("Credits must be greater than or equal to 0.")
                .LessThanOrEqualTo(10).WithMessage("Credits must be less than or equal to 10.");

            RuleFor(x => x.Description)
                .MaximumLength(5000).WithMessage("Description must be less than 5000 characters.")
                .When(x => !string.IsNullOrEmpty(x.Description));
        }
    }
}