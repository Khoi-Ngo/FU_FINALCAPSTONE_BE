using AISEA.ApiService.SHARED.DTOs.Requests;
using FluentValidation;

namespace AISEA.ApiService.BAL.Validators.Role;

public class UpdateRoleRequestValidator : AbstractValidator<UpdateRoleRequest>
{
    public UpdateRoleRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Matches(@"^[^@]*$").WithMessage("Name cannot contain '@' character.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .Matches(@"^[^@]*$").WithMessage("Description cannot contain '@' character.");
    }
}