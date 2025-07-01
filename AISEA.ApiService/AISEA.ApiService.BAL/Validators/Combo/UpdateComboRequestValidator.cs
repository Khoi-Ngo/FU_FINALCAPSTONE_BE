using AISEA.ApiService.SHARED.DTOs.Requests.Combo;
using FluentValidation;

namespace AISEA.ApiService.BAL.Validators.Combo
{
    public class UpdateComboRequestValidator : AbstractValidator<UpdateComboRequest>
    {
        public UpdateComboRequestValidator()
        {
            RuleFor(x => x.ComboName)
                .NotEmpty().WithMessage("Combo name is required.")
                .MaximumLength(255).WithMessage("Combo name must be less than 255 characters.");

            RuleFor(x => x.ComboDescription)
                .MaximumLength(1000).WithMessage("Combo description must be less than 1000 characters.")
                .When(x => !string.IsNullOrEmpty(x.ComboDescription));
        }
    }
}