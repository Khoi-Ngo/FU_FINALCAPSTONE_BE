using AISEA.ApiService.SHARED.DTOs.Requests.SystemProfile;
using FluentValidation;

namespace AISEA.ApiService.BAL.Validators.SystemProfile
{
    public class CreateStaffProfileRequestValidator : AbstractValidator<CreateStaffProfileRequest>
    {
        public CreateStaffProfileRequestValidator()
        {
            RuleFor(x => x.Campus)
                .NotEmpty().WithMessage("Campus is required.")
                .MaximumLength(255).WithMessage("Campus must be less than 255 characters.");

            RuleFor(x => x.Department)
                .NotEmpty().WithMessage("Department is required.")
                .MaximumLength(255).WithMessage("Department must be less than 255 characters.");

            RuleFor(x => x.Position)
                .NotEmpty().WithMessage("Position is required.")
                .MaximumLength(255).WithMessage("Position must be less than 255 characters.");

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("UserId must be greater than 0.");

            RuleFor(x => x.EndWorkAt)
                .GreaterThan(x => x.StartWorkAt)
                .When(x => x.EndWorkAt.HasValue && x.StartWorkAt.HasValue)
                .WithMessage("EndWorkAt must be after StartWorkAt.");
        }
    }
}