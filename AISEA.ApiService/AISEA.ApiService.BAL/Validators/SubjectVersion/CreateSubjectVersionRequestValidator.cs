using AISEA.ApiService.SHARED.DTOs.Requests.SubjectVersion;
using FluentValidation;

namespace AISEA.ApiService.BAL.Validators.SubjectVersion
{
    public class CreateSubjectVersionRequestValidator : AbstractValidator<CreateSubjectVersionRequest>
    {
        public CreateSubjectVersionRequestValidator()
        {
            RuleFor(x => x.SubjectId)
                .GreaterThan(0)
                .WithMessage("Subject ID must be greater than 0.");

            RuleFor(x => x.VersionCode)
                .NotEmpty()
                .WithMessage("Version code is required.")
                .MaximumLength(20)
                .WithMessage("Version code must not exceed 20 characters.")
                .Matches(@"^[a-zA-Z0-9._-]+$")
                .WithMessage("Version code can only contain letters, numbers, dots, underscores, and hyphens.");

            RuleFor(x => x.VersionName)
                .NotEmpty()
                .WithMessage("Version name is required.")
                .MaximumLength(255)
                .WithMessage("Version name must not exceed 255 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(2000)
                .WithMessage("Description must not exceed 2000 characters.");

            RuleFor(x => x.EffectiveFrom)
                .NotEmpty()
                .WithMessage("Effective from date is required.");

            RuleFor(x => x.EffectiveTo)
                .GreaterThan(x => x.EffectiveFrom)
                .When(x => x.EffectiveTo.HasValue)
                .WithMessage("Effective to date must be after effective from date.");

            RuleFor(x => x)
                .Must(x => x.EffectiveFrom <= DateTime.UtcNow.AddYears(10))
                .WithMessage("Effective from date cannot be more than 10 years in the future.");
        }
    }
}
