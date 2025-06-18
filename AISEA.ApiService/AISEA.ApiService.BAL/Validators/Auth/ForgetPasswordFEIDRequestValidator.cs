using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISEA.ApiService.SHARED.DTOs.Requests.Auth;
using FluentValidation;

namespace AISEA.ApiService.BAL.Validators.Auth
{
    public class ForgetPasswordFEIDRequestValidator : AbstractValidator<ForgetPasswordFEIDRequest>
    {
        public ForgetPasswordFEIDRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.VerificationCode)
                .NotEmpty().WithMessage("Verification code is required.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(6).WithMessage("New password must be at least 6 characters long.");
        }
    }
}