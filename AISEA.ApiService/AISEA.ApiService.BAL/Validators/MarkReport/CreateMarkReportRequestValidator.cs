using AISEA.ApiService.SHARED.DTOs.Requests.MarkReport;
using FluentValidation;

namespace AISEA.ApiService.BAL.Validators.MarkReport;

public class CreateMarkReportRequestValidator : AbstractValidator<CreateMarkReportRequest>
{
    public CreateMarkReportRequestValidator()
    {
        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required.")
            .MaximumLength(100).WithMessage("Category cannot exceed 100 characters.");

        RuleFor(x => x.Weight)
            .InclusiveBetween(0, 100)
            .WithMessage("Weight must be between 0 and 100.");

        RuleFor(x => x.MinScore)
            .InclusiveBetween(0, 10)
            .WithMessage("MinScore must be between 0 and 10.");

        RuleFor(x => x.Score)
            .InclusiveBetween(0, 10)
            .WithMessage("Score must be between 0 and 10.");
    }
}
