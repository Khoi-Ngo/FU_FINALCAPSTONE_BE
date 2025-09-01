using AISEA.ApiService.SHARED.DTOs.Requests.User;
using FluentValidation;

namespace AISEA.ApiService.BAL.Validators.User
{
    public class GetUsersWithSearchRequestValidator : AbstractValidator<GetUsersWithSearchRequest>
    {
        public GetUsersWithSearchRequestValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .WithMessage("Page number must be greater than 0");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .LessThanOrEqualTo(100)
                .WithMessage("Page size must be between 1 and 100");

            RuleFor(x => x.Search)
                .MaximumLength(100)
                .WithMessage("Search term cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.Search));
        }
    }
}
