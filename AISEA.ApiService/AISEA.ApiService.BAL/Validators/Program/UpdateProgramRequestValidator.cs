using AISEA.ApiService.SHARED.DTOs.Requests.Program;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AISEA.ApiService.BAL.Validators.Program
{
    public class UpdateProgramRequestValidator : AbstractValidator<UpdateProgramRequest>
    {
        public UpdateProgramRequestValidator()
        {
            RuleFor(x => x.ProgramCode)
                .NotEmpty().WithMessage("Program code is required.")
                .MaximumLength(50).WithMessage("Program code must be less than 50 characters.")
                .Matches(@"^[A-Z0-9_\-]+$").WithMessage("Program code must contain only uppercase letters, numbers, underscores, and hyphens.");

            RuleFor(x => x.ProgramName)
                .NotEmpty().WithMessage("Program name is required.")
                .MaximumLength(255).WithMessage("Program name must be less than 255 characters.");
        }
    }
}
