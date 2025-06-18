using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISEA.ApiService.SHARED.DTOs.Requests.Role;
using FluentValidation;

namespace AISEA.ApiService.BAL.Validators.Role;

public class CreateRoleRequestValidator : AbstractValidator<CreateRoleRequest>
{
    public CreateRoleRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Matches(@"^[a-zA-Z0-9\s]+$").WithMessage("Name must contain only letters, numbers, and spaces.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .Matches(@"^[a-zA-Z0-9\s]+$").WithMessage("Description must contain only letters, numbers, and spaces.");
    }
}