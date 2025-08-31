using AISEA.ApiService.SHARED.DTOs.Requests.SubjectComment;
using AISEA.ApiService.SHARED.Const.Enums;
using FluentValidation;

namespace AISEA.ApiService.BAL.Validators.SubjectComment
{
    public class GetSubjectCommentsRequestValidator : AbstractValidator<GetSubjectCommentsRequest>
    {
        public GetSubjectCommentsRequestValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .WithMessage("Page number must be greater than 0");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .LessThanOrEqualTo(100)
                .WithMessage("Page size must be between 1 and 100");

            RuleFor(x => x.SortBy)
                .IsInEnum()
                .WithMessage("Invalid sort field");

            RuleFor(x => x.SortDirection)
                .IsInEnum()
                .WithMessage("Invalid sort direction");
        }
    }
}
