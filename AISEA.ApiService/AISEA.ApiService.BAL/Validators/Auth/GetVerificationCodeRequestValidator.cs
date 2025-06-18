using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISEA.ApiService.SHARED.DTOs.Requests.Auth;
using FluentValidation;

namespace AISEA.ApiService.BAL.Validators.Auth
{
    public class GetVerificationCodeRequestValidator : AbstractValidator<GetVerificationCodeRequest>
    {
        public GetVerificationCodeRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
        }
    }
}